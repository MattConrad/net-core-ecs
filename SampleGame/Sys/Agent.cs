using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Agent
    {
        public static CombatChoicesAndTargets GetPossibleActions(EcsRegistrar rgs, long globalId, long agentId, List<long> battlefieldEntityIds)
        {
            var possibleCombatActions = new CombatChoicesAndTargets();

            var naturalWeaponsMap = rgs.GetPartSingle<Parts.NaturalWeaponsMap>(globalId);

            var agentAnatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId);

            //var agentWieldingDisabled = agentAnatomyModifers
            //    .Where(m => m.EquipmentSlotDisabled == Vals.BodyEquipmentSlots.WieldObjectAppendage)
            //    .Select(m => m.EquipmentSlotDisabled)
            //    .ToList();

            var agentWieldedIds = agentAnatomy.SlotsEquipped
                .Where(s => s.Key == Vals.BodySlots.WieldHandLeft || s.Key == Vals.BodySlots.WieldHandRight || s.Key == Vals.BodySlots.WieldTwoHanded)
                .Select(s => s.Value)
                .ToList();
            var agentWieldedIdToWeaponEntityName = agentWieldedIds
                .Where(id => id != 0)
                .Distinct()
                .Select(id => new { Id = id, Name = rgs.GetPartSingle<Parts.EntityName>(id) } )
                .ToDictionary(n => n.Id, n => n.Name);

            var agentNaturalWeaponSlots = naturalWeaponsMap.NaturalWeaponSets[agentAnatomy.NaturalWeaponsCategory];
            //MWCTODO: some magic happens here and we find out a wristblade or a laser eye has been equipped, and add that to the slots.
            // also any quickslots that might have attack items in them. we want quickslots. maybe a wristblade or a laser eye automatically adds a new, fixed quickslot for itself? i like this idea upon first think!
            var recordedIds = new HashSet<long>();
            foreach (var slot in agentNaturalWeaponSlots.Keys)
            {
                var weaponId = agentAnatomy.SlotsEquipped[slot];
                if (weaponId == 0L) weaponId = agentNaturalWeaponSlots[slot];

                if (recordedIds.Contains(weaponId)) continue;

                var weapon = Bundle.GetWeaponBundle(rgs, weaponId);
                var choice = new AgentActionChoice
                {
                    AgentId = agentId,
                    Category = Vals.CombatCategory.TopLevelAction,
                    Description = $"Melee {weapon.EntityName.GeneralName}",
                    Action = Vals.CombatAction.AttackWeaponMelee,
                    WeaponBodySlot = slot,
                    WeaponEntityId = weaponId,
                    NextCategory = Vals.CombatCategory.MeleeTarget
                };

                possibleCombatActions.Choices.Add(choice);
                recordedIds.Add(weaponId);
            }

            foreach (long id in battlefieldEntityIds)
            {
                if (id == agentId) continue;

                var entityName = rgs.GetPartSingleOrDefault<Parts.EntityName>(id);
                var entityAgent = rgs.GetPartSingleOrDefault<Parts.Agent>(id);

                if (entityName == null || entityAgent == null) continue;

                if (entityAgent.CombatStatusTags.Intersect(Vals.CombatStatusTag.CombatTerminalStatuses).Any()) continue;

                possibleCombatActions.MeleeTargets.Add(id, entityName.ProperName);
            }

            //add stances
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.AllStances,
                Description = "Stance (Defensive)",
                Action = Vals.CombatAction.StanceDefensive
            });
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.AllStances,
                Description = "Stance (Stand Ground)",
                Action = Vals.CombatAction.StanceStandGround
            });
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.AllStances,
                Description = "Stance (Aggressive)",
                Action = Vals.CombatAction.StanceAggressive
            });

            //add top-level choices that we know are present: note that we already did NextCategory = primary-melee because the description there is context dependent.
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.TopLevelAction,
                Description = "All Melee Options",
                NextCategory = Vals.CombatCategory.AllMelee
            });
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.TopLevelAction,
                Description = "Change Stance",
                NextCategory = Vals.CombatCategory.AllStances
            });
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.TopLevelAction,
                Description = "Run Away",
                Action = Vals.CombatAction.RunAway
            });
            possibleCombatActions.Choices.Add(new AgentActionChoice
            {
                AgentId = agentId,
                Category = Vals.CombatCategory.TopLevelAction,
                Description = "Do Nothing",
                Action = Vals.CombatAction.DoNothing
            });


            return possibleCombatActions;
        }

        internal static ActionChosen GetAgentAICombatAction(EcsRegistrar rgs, long globalId, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Vals.AI.MeleeOnly) return AICombatActionMeleeOnly(rgs, globalId, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        private static ActionChosen AICombatActionMeleeOnly(EcsRegistrar rgs, long globalId, long attackerId, List<long> battlefieldEntityIds)
        {
            var action = new ActionChosen { Action = Vals.CombatAction.DoNothing, AgentId = attackerId };

            var naturalWeaponsMap = rgs.GetPartSingle<Parts.NaturalWeaponsMap>(globalId);

            var targetIdsToWeights = battlefieldEntityIds
                .Select(id => new { Id = id, Agent = rgs.GetPartSingleOrDefault<Parts.Agent>(id) })
                .Where(a => a.Id != attackerId && a.Agent != null && !Vals.CombatStatusTag.CombatTerminalStatuses.Intersect(a.Agent.CombatStatusTags).Any())
                .ToDictionary(a => a.Id, a => 0);

            var attackerFaction = rgs.GetPartSingle<Parts.Faction>(attackerId);
            var factionInteractions = rgs.GetPartSingle<Parts.FactionInteractionSet>(globalId);

            //eventually we have to figure out how to weight faction relative to everything else.
            var possibleTargetIds = targetIdsToWeights.Keys.ToArray();
            foreach(var possibleTargetId in possibleTargetIds)
            {
                var targetFaction = rgs.GetPartSingle<Parts.Faction>(possibleTargetId);
                targetIdsToWeights[possibleTargetId] = factionInteractions.Interactions
                    .SingleOrDefault(i => i.SourceFaction == attackerFaction.CurrentPublicFaction && i.TargetFaction == targetFaction.CurrentPublicFaction)
                    ?.Disposition ?? 0;
            }

            var minWeight = targetIdsToWeights.Values.Any() ? targetIdsToWeights.Values.Min() : 0;

            if (minWeight < 0)
            {
                var targetId = targetIdsToWeights.First(kvp => kvp.Value == minWeight).Key;

                var attackerAnatomy = rgs.GetPartSingle<Parts.Anatomy>(attackerId);

                //MWCTODO++: yaaaah this is way too human centric. 
                var weaponId = attackerAnatomy.SlotsEquipped.GetValueOrDefault(Vals.BodySlots.WieldHandRight, 0);
                if (weaponId == 0) weaponId = attackerAnatomy.SlotsEquipped.GetValueOrDefault(Vals.BodySlots.WieldHandLeft, 0);

                if (weaponId == 0)
                {
                    var nwmap = naturalWeaponsMap.NaturalWeaponSets.GetValueOrDefault(attackerAnatomy.BodyPlan, null);

                    if (nwmap != null && nwmap.Keys.Any())
                    {
                        //MWCTODO: and this isn't great.
                        weaponId = nwmap.First().Value;
                    }
                }

                action.Action = Vals.CombatAction.AttackWeaponMelee;
                action.TargetEntityId = targetId;
                action.WeaponEntityId = weaponId;
            }

            return action;
        }
    }
}
