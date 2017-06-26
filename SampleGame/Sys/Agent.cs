﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            var recordedIds = new HashSet<long>();
            //whatever id is wielded in index 0 is the main hand. for now, everything else is the offhand. octopodes have a lot of offhands for now.
            for (int i = 0; i < agentWieldedIds.Count; i++)
            {
                //2 hand weapons shouldn't show up repeatedly. for now at least, neither will multiple punches.
                if (recordedIds.Contains(agentWieldedIds[i])) continue;

                string attackType = Vals.CombatAction.AttackWeaponMelee;

                //MWCTODO+++: don't default to "punch", get the natural weapon type from the anatomy, maybe with a lookup. 
                string weaponName = (agentWieldedIds[i] == 0) ? "punch" : agentWieldedIdToWeaponEntityName[agentWieldedIds[i]].GeneralName;

                //MWCTODO+: this shouldn't default to "hand", but whatever the wielding appendage is called.
                string mainOrOffhand = (i == 0) ? "main hand" : "offhand";

                var choice = new AgentActionChoice
                {
                    AgentId = agentId,
                    Category = Vals.CombatCategory.AllMelee,
                    Description = $"Melee {weaponName} ({mainOrOffhand})",
                    Action = attackType,
                    WeaponHandIndex = i,
                    WeaponEntityId = agentWieldedIds[i],
                    NextCategory = Vals.CombatCategory.MeleeTarget
                };

                possibleCombatActions.Choices.Add(choice);

                if (i == 0)
                {
                    var primaryChoice = new AgentActionChoice
                    {
                        AgentId = agentId,
                        Category = Vals.CombatCategory.TopLevelAction,
                        Description = $"Primary Melee {weaponName} ({mainOrOffhand})",
                        Action = attackType,
                        WeaponHandIndex = i,
                        WeaponEntityId = agentWieldedIds[i],
                        NextCategory = Vals.CombatCategory.MeleeTarget
                    };

                    possibleCombatActions.Choices.Add(primaryChoice);
                }

                recordedIds.Add(agentWieldedIds[i]);
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

        //NOTE: see notes above, but this eventually won't be limited to wielding hands.
        //internal static long GetNaturalWeapon(Parts.NaturalWeaponsMap map, Parts.Anatomy agentAnatomy, string slotName)
        //{
        //    if (agentAnatomy.BodyPlan == Vals.BodyPlan.Human && slotName == Vals.BodyEquipmentSlots.WieldObjectAppendage)
        //    {
        //        return map.NameToNaturalWeaponId[Vals.NaturalWeaponNames.HumanPunch];
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        internal static ActionChosen GetAgentAICombatAction(EcsRegistrar rgs, long globalId, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Vals.AI.MeleeOnly) return CombatActionMeleeOnly(rgs, globalId, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        private static ActionChosen CombatActionMeleeOnly(EcsRegistrar rgs, long globalId, long attackerId, List<long> battlefieldEntityIds)
        {
            var action = new ActionChosen { Action = Vals.CombatAction.DoNothing, AgentId = attackerId };

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

                action.Action = Vals.CombatAction.AttackWeaponMelee;
                action.TargetEntityId = targetId;
            }

            return action;
        }
    }
}
