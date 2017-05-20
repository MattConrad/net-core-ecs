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

        internal static string GetAgentCombatAction(EcsRegistrar rgs, Parts.FactionInteractionSet factionInteractions, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Vals.AI.MeleeOnly) return CombatActionMeleeOnly(rgs, factionInteractions, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        //MWCTODO: ugh, faction interactions.
        private static string CombatActionMeleeOnly(EcsRegistrar rgs, Parts.FactionInteractionSet factionInteractions, long attackerId, List<long> battlefieldEntityIds)
        {
            string action = Vals.CombatActions.DoNothing;
            var factionDeltas = new Dictionary<long, int>();

            var attackerFaction = rgs.GetParts<Parts.Faction>(attackerId).SingleOrDefault();


            //this will do for now, but a) faction shouldn't be as important as perceived danger level, b) this is busier than maybe it should be, and c) can you really perceive faction by looking at someone?
            // i am not sure I want numeric factions at all any more. seemed tidy--but are they really useful?
            if (attackerFaction != null)
            {
                foreach(long entityId in battlefieldEntityIds)
                {
                    if (entityId == attackerId) continue;

                    var entityFactionReputations = rgs.GetParts<Parts.Faction>(entityId).SingleOrDefault()?.PublicFactionReputations;
                    if (entityFactionReputations == null) continue;

                    int delta = attackerFaction
                        .PublicFactionReputations
                        .Select(kvp => (entityFactionReputations.ContainsKey(kvp.Key) ? kvp.Value - entityFactionReputations[kvp.Key] : 0))
                        .Sum(d => Math.Abs(d));

                    factionDeltas.Add(entityId, delta);
                }
            }

            if (factionDeltas.Any())
            {
                var targetId = factionDeltas.First(kvp => kvp.Value == factionDeltas.Values.Max()).Key;

                return Vals.CombatActions.AttackMelee + " " + targetId;
            }
            else
            {
                return action;
            }
        }
    }
}
