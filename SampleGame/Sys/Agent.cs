using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Agent
    {
        internal static string GetCombatAction(EcsRegistrar rgs, string attackerAI, long attackerId, List<long> battlefieldEntityIds)
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
