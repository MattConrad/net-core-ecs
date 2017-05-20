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
            var factionInteractions = rgs.GetPartSingle<Parts.FactionInteractionSet>(globalId);

            while (true)
            {
                var battlefieldContainer = rgs.GetPartSingle<Parts.Container>(battlefieldId);
                var battlefieldEntityIds = battlefieldContainer.EntityIds.ToList();

                foreach(var agentId in battlefieldEntityIds)
                {
                    var battlefieldAgent = rgs.GetParts<Parts.Agent>(agentId).SingleOrDefault();
                    if (battlefieldAgent == null) continue;
                    //probably we should never return output for "Dead", but we might want to return "zzzz" for "Sleeping" or similar.
                    if (battlefieldAgent.CombatStatusTags.Intersect(Vals.CombatStatusTag.IncapacitatingStatuses).Any()) continue;

                    string agentActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Vals.AI.PlayerChoice)
                    {
                        var possibleActions = Agent.GetPossibleActions(rgs, agentId, battlefieldEntityIds);
                        agentActionSet = receivePlayerInput(possibleActions);
                    }
                    else
                    {
                        agentActionSet = Agent.GetAgentCombatAction(rgs, factionInteractions, battlefieldAgent.ActiveCombatAI, agentId, battlefieldEntityIds);
                    }

                    //we cheat a bit here, with knowledge that won't stay true forever.
                    string[] agentActionStrings = agentActionSet.Split(' ');
                    long? targetId = agentActionStrings.Length > 1 ? long.Parse(agentActionStrings[1]) : (long?)null;

                    var results = Combat.ProcessAgentAction(rgs, agentId, targetId, agentActionStrings[0]);

                    var output = Narrator.OutputForCombatMessages(rgs, results);

                    yield return output;
                }

                var endOfBattleOutput = new Output() { Category = OutputCategory.Text };
                if (!battlefieldEntityIds.Any())
                {
                    endOfBattleOutput.Data = "Somehow everyone died. Oops.";
                    yield return new List<Output> { endOfBattleOutput };
                    yield break;
                }
                else if (battlefieldEntityIds.Count == 1)
                {
                    endOfBattleOutput.Data = "One man^H^H^H entity standing. The end.";
                    yield return new List<Output> { endOfBattleOutput };
                    yield break;
                }

            }
        }


    }
}
