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

                foreach(var agentId in battlefieldEntityIds)
                {
                    var battlefieldAgent = rgs.GetParts<Parts.Agent>(agentId).SingleOrDefault();
                    if (battlefieldAgent == null) continue;

                    string agentActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Parts.Agent.Vals.AI.PlayerChoice)
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
                        if (result.TargetCondition <= 0) battlefieldContainer.EntityIds.Remove(result.TargetId);
                    }

                    var output = Narrator.OutputForCombatMessages(rgs, results);

                    yield return output;

                    //MWCTODO: ummm, this needs to be a LITTLE fancier.
                    bool combatFinished = !battlefieldContainer.EntityIds.Any();

                    if (combatFinished) yield break;
                }
            }
        }
    }
}
