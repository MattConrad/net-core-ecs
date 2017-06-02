using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Battlefield
    {
        internal static IEnumerable<List<Output>> RunBattlefield(EcsRegistrar rgs, long globalId, long battlefieldId, Func<Dictionary<string, string>, string> receivePlayerInput)
        {
            while (true)
            {
                var battlefieldContainer = rgs.GetPartSingle<Parts.Container>(battlefieldId);
                var battlefieldEntityIds = battlefieldContainer.EntityIds.ToList();

                foreach(var agentId in battlefieldEntityIds)
                {
                    var battlefieldAgent = rgs.GetPartSingleOrDefault<Parts.Agent>(agentId);

                    if (battlefieldAgent == null) continue;
                    if (battlefieldAgent.CombatStatusTags.Intersect(Vals.CombatStatusTag.CombatTerminalStatuses).Any()) continue;

                    string agentActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Vals.AI.PlayerChoice)
                    {
                        var possibleActions = Agent.GetPossibleActions(rgs, agentId, battlefieldEntityIds);
                        agentActionSet = receivePlayerInput(possibleActions);
                    }
                    else
                    {
                        agentActionSet = Agent.GetAgentAICombatAction(rgs, globalId, battlefieldAgent.ActiveCombatAI, agentId, battlefieldEntityIds);
                    }

                    //we cheat a bit here, with knowledge that won't stay true forever.
                    string[] agentActionStrings = agentActionSet.Split(' ');
                    long? targetId = agentActionStrings.Length > 1 ? long.Parse(agentActionStrings[1]) : (long?)null;

                    var results = Combat.ProcessAgentAction(rgs, agentId, targetId, agentActionStrings[0]);

                    var output = Narrator.OutputForCombatMessages(rgs, results);

                    yield return output;
                }

                var endOfBattleOutputs = GetEndOfBattleOutputs(rgs, battlefieldEntityIds);
                if (endOfBattleOutputs.Any())
                {
                    yield return endOfBattleOutputs;
                    yield break;
                }
            }
        }

        internal static List<Output> GetEndOfBattleOutputs(EcsRegistrar rgs, List<long> battlefieldEntityIds)
        {
            var outputs = new List<Output>();

            var nonCombatTerminals = battlefieldEntityIds
                .Select(id => new { Id = id, Agent = rgs.GetPartSingleOrDefault<Parts.Agent>(id) })
                .Where(a => a.Agent != null && !a.Agent.CombatStatusTags.Intersect(Vals.CombatStatusTag.CombatTerminalStatuses).Any());

            var distinctNctFactions = nonCombatTerminals
                .Select(a => rgs.GetPartSingleOrDefault<Parts.Faction>(a.Id)?.CurrentPublicFaction ?? Vals.FactionName.Unaffiliated)
                .Distinct()
                .ToList();

            //this needs to hook into the narrator.
            var output = new Output() { Category = OutputCategory.Text };
            if (!distinctNctFactions.Any())
            {
                output.Data = "Somehow everyone died. Oops!";
                outputs.Add(output);
            }
            else if (distinctNctFactions.Count == 1)
            {
                output.Data = $"Combat is over. {distinctNctFactions.First()} win.";
                outputs.Add(output);
            }

            return outputs;
        }


    }
}
