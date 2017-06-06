using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Agent
    {
        //MWCTODO+: for now these are fixed, but later they'll vary with context (drop your weapon and you're punching not slashing).
        public static Dictionary<string, string> GetPossibleActions(EcsRegistrar rgs, long agentId, List<long> battlefieldEntityIds)
        {
            var targetActions = new Dictionary<string, string>();
            //this is defective for many reasons. the general Dictionary<string, string> concept isn't really good, but 
            // relying on entity proper name and those all being unique, no not good at all.
            foreach(long id in battlefieldEntityIds)
            {
                if (id == agentId) continue;

                var entityName = rgs.GetPartSingle<Parts.EntityName>(id);
                var entityAgent = rgs.GetPartSingle<Parts.Agent>(id);

                if (entityAgent.CombatStatusTags.Intersect(Vals.CombatStatusTag.CombatTerminalStatuses).Any()) continue;
                
                targetActions.Add($"Attack Melee {entityName.ProperName}", $"{Vals.CombatAction.AttackWeaponMelee} {id}");
            }

            var standardActions = new Dictionary<string, string>
            {
                ["Switch To AI (for testing)"] = Vals.CombatAction.SwitchToAI,
                ["Stance (Defensive)"] = Vals.CombatAction.StanceDefensive,
                ["Stance (Stand Ground)"] = Vals.CombatAction.StanceStandGround,
                ["Stance (Aggressive)"] = Vals.CombatAction.StanceAggressive,
                ["Doooo Nothing"] = Vals.CombatAction.DoNothing
            };

            return targetActions
                .Select(d => d)
                .Union(standardActions.Select(d => d))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static PossibleCombatActions GetPossibleActions2(EcsRegistrar rgs, long agentId, List<long> battlefieldEntityIds)
        {
            var possibleCombatActions = new PossibleCombatActions();

            var agentAnatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId);
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

                //MWCTODO+: don't default to "punch", get the natural weapon type from the anatomy, maybe with a lookup. 
                string weaponName = (agentWieldedIds[i] == 0) ? "punch" : agentWieldedIdToEntityName[agentWieldedIds[i]].GeneralName;

                //MWCTODO+: this shouldn't default to "hand", but whatever the wielding appendage is called.
                string mainOrOffhand = (i == 0) ? "main hand" : "offhand";

                var choice = new ActorChoicesPossible
                {
                    ActionWeaponOrSpellName = weaponName,
                    ActionModifier = mainOrOffhand,
                    Action = attackType,
                    WeaponHandIndex = i,
                    WeaponEntityId = agentWieldedIds[i],
                    NextLevelCategory = true
                };

                possibleCombatActions.SingleTargetMeleeAttacks.Add(choice);

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

            var standardActions = new Dictionary<string, string>
            {
                ["Switch To AI (for testing)"] = Vals.CombatAction.SwitchToAI,
                ["Stance (Defensive)"] = Vals.CombatAction.StanceDefensive,
                ["Stance (Stand Ground)"] = Vals.CombatAction.StanceStandGround,
                ["Stance (Aggressive)"] = Vals.CombatAction.StanceAggressive,
                ["Doooo Nothing"] = Vals.CombatAction.DoNothing
            };

            foreach(string actionText in standardActions.Keys)
            {
                possibleCombatActions.NonAttackActions.Add(new ActorChoicesPossible { ActionWeaponOrSpellName = actionText, Action = standardActions[actionText] });
            }

            return possibleCombatActions;
        }

        internal static string GetAgentAICombatAction(EcsRegistrar rgs, long globalId, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Vals.AI.MeleeOnly) return CombatActionMeleeOnly(rgs, globalId, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        private static string CombatActionMeleeOnly(EcsRegistrar rgs, long globalId, long attackerId, List<long> battlefieldEntityIds)
        {
            string action = Vals.CombatAction.DoNothing;

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

                action = Vals.CombatAction.AttackWeaponMelee + " " + targetId;
            }

            return action;
        }
    }
}
