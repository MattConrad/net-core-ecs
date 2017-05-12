using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Agent
    {
        //MWCTODO: this step should also include piping the current permitted actions out to the player.
        // for now these are fixed, but later they'll vary with context (drop your weapon and you're punching not slashing).
        public static Dictionary<string, string> GetPossibleActions(EcsRegistrar rgs, long agentId, List<long> battlefieldEnttiyIds)
        {
            var targetActions = new Dictionary<string, string>();
            //this is defective for many reasons. the general Dictionary<string, string> concept isn't really good, but 
            // relying on entity proper name and those all being unique, no not good at all.
            foreach(long id in battlefieldEnttiyIds)
            {
                if (id == agentId) continue;

                var entityName = rgs.GetPartsOfType<Parts.EntityName>(id).Single();
                targetActions.Add($"Attack Melee {entityName.ProperName}", $"{Combat.Actions.AttackMelee} {id}");
            }

            var standardActions = new Dictionary<string, string>
            {
                ["Switch To AI (for testing)"] = Sys.Combat.Actions.SwitchToAI,
                ["Stance (Defensive)"] = Sys.Combat.Actions.StanceDefensive,
                ["Stance (Stand Ground)"] = Sys.Combat.Actions.StanceStandGround,
                ["Stance (Aggressive)"] = Sys.Combat.Actions.StanceAggressive
            };

            return targetActions
                .Select(d => d)
                .Union(standardActions.Select(d => d))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static string GetAgentCombatAction(EcsRegistrar rgs, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
        {
            if (attackerAI == Parts.Agent.Vals.AI.MeleeOnly) return CombatActionMeleeOnly(rgs, attackerId, battlefieldEntityIds);

            throw new ArgumentException($"Attacker AI {attackerAI} not supported.");
        }

        private static string CombatActionMeleeOnly(EcsRegistrar rgs, long attackerId, List<long> battlefieldEntityIds)
        {
            string action = "do-nothing";
            var factionDeltas = new Dictionary<long, int>();

            var attackerFaction = rgs.GetPartsOfType<Parts.Faction>(attackerId).SingleOrDefault();

            //this will do for now, but a) faction shouldn't be as important as perceived danger level, b) this is busier than maybe it should be, and c) can you really perceive faction by looking at someone?
            // i am not sure I want numeric factions at all any more. seemed tidy--but are they really useful?
            if (attackerFaction != null)
            {
                foreach(long entityId in battlefieldEntityIds)
                {
                    if (entityId == attackerId) continue;

                    var entityFactionReputations = rgs.GetPartsOfType<Parts.Faction>(entityId).SingleOrDefault()?.FactionReputations;
                    if (entityFactionReputations == null) continue;

                    int delta = attackerFaction
                        .FactionReputations
                        .Select(kvp => (entityFactionReputations.ContainsKey(kvp.Key) ? kvp.Value - entityFactionReputations[kvp.Key] : 0))
                        .Sum(d => Math.Abs(d));

                    factionDeltas.Add(entityId, delta);
                }
            }

            var targetId = factionDeltas.First(kvp => kvp.Value == factionDeltas.Values.Max()).Key;

            return Combat.Actions.AttackMelee + " " + targetId;
        }
    }
}
