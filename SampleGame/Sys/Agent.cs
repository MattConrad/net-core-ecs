using System;
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
            var agentAnatomyModifers = rgs.GetParts<Parts.AnatomyModifier>(agentId);

            //MWCTODO: uuuuugh. ok, I screwed up. this can't work, because we really need to know WHICH hand was disabled.
            // there is no way to  have one equipement slot disabled and sometimes be shield hand and sometimes main hand.
            // we can say disabled is always shield hand first, or always main hand first, but not either one or the other.
            // i don't remember what I was thinking. maybe disabled is always main hand first, but that still sucks.
            // i thought it would be a pain in the ass to check both left ring finger and right ring finger, and it would have been, 
            // but that would have worked, and this won't.
            // i think i will rewrite this with the bitmask approach--that is interesting, anyway, but i'm not going to do that
            // right now, we'll redo it later and ignore the messed up part here for a while.
            var agentWieldingDisabled = agentAnatomyModifers
                .Where(m => m.EquipmentSlotDisabled == Vals.BodyEquipmentSlots.WieldObjectAppendage)
                .Select(m => m.EquipmentSlotDisabled)
                .ToList();

            //MWCTODO+: yanno, there could be other natural weapons not in wielding hands, like bites/kicks.
            // and you could have, conceivably, a flame-hand gauntlet or a laser eye in your eyesocket
            // we probably ought to do all slots and not just wieldeds.
            var agentWieldedIds = agentAnatomy.SlotsEquipped
                .Where(s => s.Key == Vals.BodyEquipmentSlots.WieldObjectAppendage)
                .Select(s => s.Value)
                .ToList();
            var agentWieldedIdToEntityName = agentWieldedIds
                .Where(id => id != 0)
                .Distinct()
                .Select(id => new { Id = id, Name = rgs.GetPartSingle<Parts.EntityName>(id) } )
                .ToDictionary(n => n.Id, n => n.Name);

            var recordedIds = new HashSet<long>();
            //whatever id is wielded in index 0 is the main hand. for now, everything else is the offhand. octopodes have a lot of offhands for now.
            for (int i = 0; i < agentWieldedIds.Count; i++)
            {
                //2 hand weapons shouldn't show up repeatedly.
                //MWCTODO++: but maybe 2 empty hands with offhand punch should....
                if (recordedIds.Contains(agentWieldedIds[i])) continue;

                //MWCTODO: this gets fancier when we introduce ranged weaponry. we'll need to check if there are any enemies in range, for one thing.
                string attackType = Vals.CombatAction.AttackWeaponMelee;

                //MWCTODO+++: don't default to "punch", get the natural weapon type from the anatomy, maybe with a lookup. 
                string weaponName = (agentWieldedIds[i] == 0) ? "punch" : agentWieldedIdToEntityName[agentWieldedIds[i]].GeneralName;

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

                //MWCTODO: again, ranged weaponry will change things.
                //MWCTODO+: proper name is not correct here (hardly anywhere we're using it, really . . .)
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
        internal static long GetNaturalWeapon(Parts.NaturalWeaponsMap map, Parts.Anatomy agentAnatomy, string slotName)
        {
            if (agentAnatomy.BodyPlan == Vals.BodyPlan.Human && slotName == Vals.BodyEquipmentSlots.WieldObjectAppendage)
            {
                return map.NameToNaturalWeaponId[Vals.NaturalWeaponNames.HumanPunch];
            }
            else
            {
                return 0;
            }
        }

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
