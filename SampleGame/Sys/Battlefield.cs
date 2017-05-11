using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Battlefield
    {
        internal static IEnumerable<List<string>> RunBattlefield(EcsRegistrar rgs, long battlefieldId, Func<string> receivePlayerInput)
        {
            while (true)
            {
                //eventually UPDATE: real soon now!  entities may enter or leave the battlefield, so we requery every iteration.
                var battlefieldEntityIds = rgs.GetPartsOfType<Parts.Container>(battlefieldId)
                    .Single(p => p.Tag == Parts.Container.Vals.Tag.Battlefield)
                    .EntityIds
                    .ToList();

                foreach(var agentId in battlefieldEntityIds)
                {
                    var battlefieldAgent = rgs.GetPartsOfType<Parts.Agent>(agentId).SingleOrDefault();
                    if (battlefieldAgent == null) continue;

                    string agentActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Parts.Agent.Vals.AI.PlayerChoice)
                    {
                        //MWCTODO: this step should also include piping the current permitted actions out to the player.
                        // for now these are fixed, but later they'll vary with context (drop your weapon and you're punching not slashing).
                        agentActionSet = receivePlayerInput();
                    }
                    else
                    {
                        agentActionSet = Agent.GetCombatAction(rgs, battlefieldAgent.ActiveCombatAI, agentId, battlefieldEntityIds);
                    }

                    //we cheat a bit here, with knowledge that won't stay true forever.
                    string[] agentActionStrings = agentActionSet.Split(' ');
                    long? targetId = agentActionStrings.Length > 1 ? long.Parse(agentActionStrings[1]) : (long?)null;

                    bool combatFinished = false;
                    var results = Combat.ProcessAgentAction(rgs, agentId, targetId, agentActionStrings[0], out combatFinished);

                    yield return results;

                    if (combatFinished) yield break;
                }
            }
        }
    }
}
