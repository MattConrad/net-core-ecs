using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Agent
    {
        //MWCTODO: for now these are fixed, but later they'll vary with context (drop your weapon and you're punching not slashing).
        public static Dictionary<string, string> GetPossibleActions(EcsRegistrar rgs, long agentId, List<long> battlefieldEnttiyIds)
        {
            var targetActions = new Dictionary<string, string>();
            //this is defective for many reasons. the general Dictionary<string, string> concept isn't really good, but 
            // relying on entity proper name and those all being unique, no not good at all.
            foreach(long id in battlefieldEnttiyIds)
            {
                if (id == agentId) continue;

                var entityName = rgs.GetParts<Parts.EntityName>(id).Single();
                targetActions.Add($"Attack Melee {entityName.ProperName}", $"{Vals.CombatActions.AttackMelee} {id}");
            }

            var standardActions = new Dictionary<string, string>
            {
                ["Switch To AI (for testing)"] = Vals.CombatActions.SwitchToAI,
                ["Stance (Defensive)"] = Vals.CombatActions.StanceDefensive,
                ["Stance (Stand Ground)"] = Vals.CombatActions.StanceStandGround,
                ["Stance (Aggressive)"] = Vals.CombatActions.StanceAggressive
            };

            return targetActions
                .Select(d => d)
                .Union(standardActions.Select(d => d))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static string GetAgentCombatAction(EcsRegistrar rgs, long globalId, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Vals.AI.MeleeOnly) return CombatActionMeleeOnly(rgs, globalId, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        private static string CombatActionMeleeOnly(EcsRegistrar rgs, long globalId, long attackerId, List<long> battlefieldEntityIds)
        {
            string action = Vals.CombatActions.DoNothing;

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

                action = Vals.CombatActions.AttackMelee + " " + targetId;
            }

            return action;
        }
    }
}
