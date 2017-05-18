using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Battlefield
    {
        internal static IEnumerable<List<Output>> RunBattlefield(EcsRegistrar rgs, long battlefieldId, Func<Dictionary<string, string>, string> receivePlayerInput)
        {
            while (true)
            {
                var battlefieldContainer = rgs.GetPartSingle<Parts.Container>(battlefieldId);
                var battlefieldEntityIds = battlefieldContainer.EntityIds.ToList();

                //sometimes an entity gets cancelled (killed) in the middle of the loop by an earlier entity.
                // for now, that will cancel the entity's action. later, with the scheduler, it won't.
                var cancelledEntityIds = new HashSet<long>();

                foreach(var agentId in battlefieldEntityIds)
                {
                    if (cancelledEntityIds.Contains(agentId)) continue;

                    var battlefieldAgent = rgs.GetParts<Parts.Agent>(agentId).SingleOrDefault();
                    if (battlefieldAgent == null) continue;

                    string agentActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Vals.AI.PlayerChoice)
                    {
                        var possibleActions = Agent.GetPossibleActions(rgs, agentId, battlefieldEntityIds);
                        agentActionSet = receivePlayerInput(possibleActions);
                    }
                    else
                    {
                        agentActionSet = Agent.GetAgentCombatAction(rgs, battlefieldAgent.ActiveCombatAI, agentId, battlefieldEntityIds);
                    }

                    //we cheat a bit here, with knowledge that won't stay true forever.
                    string[] agentActionStrings = agentActionSet.Split(' ');
                    long? targetId = agentActionStrings.Length > 1 ? long.Parse(agentActionStrings[1]) : (long?)null;

                    //in due course this will return a list of results, not a single.
                    var results = Combat.ProcessAgentAction(rgs, agentId, targetId, agentActionStrings[0]);

                    //eventually we will leave dead bodies on the field, they just won't act any more.
                    foreach(var result in results)
                    {
                        //eventually, we have to be smarter about filtering to appropriate results
                        if (result.TargetId == 0) continue;
                        if (result.TargetCondition <= 0)
                        {
                            battlefieldContainer.EntityIds.Remove(result.TargetId);
                            cancelledEntityIds.Add(result.TargetId);
                        }
                    }

                    var output = Narrator.OutputForCombatMessages(rgs, results);

                    yield return output;
                }

                var bailoutOutput = new Output() { Category = OutputCategory.Text };
                if (!battlefieldEntityIds.Any())
                {
                    bailoutOutput.Data = "Somehow everyone died. Oops.";
                    yield return new List<Output> { bailoutOutput };
                    yield break;
                }
                else if (battlefieldEntityIds.Count == 1)
                {
                    bailoutOutput.Data = "One man^H^H^H entity standing. The end.";
                    yield return new List<Output> { bailoutOutput };
                    yield break;
                }

            }
        }

    }
}
