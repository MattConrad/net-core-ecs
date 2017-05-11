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
                //eventually, entities may enter or leave the battlefield, so we requery every iteration.
                var battlefieldEntityIds = rgs.GetPartsOfType<Parts.Container>(battlefieldId)
                    .Single(p => p.Tag == Parts.Container.Vals.Tag.Battlefield)
                    .EntityIds
                    .ToList();

                foreach(var id in battlefieldEntityIds)
                {
                    var battlefieldAgent = rgs.GetPartsOfType<Parts.Agent>(id).SingleOrDefault();
                    if (battlefieldAgent == null) continue;

                    string nextActionSet;
                    if (battlefieldAgent.ActiveCombatAI == Parts.Agent.Vals.AI.PlayerChoice)
                    {
                        //MWCTODO: this step should also include piping the current permitted actions out to the player.
                        // for now these are fixed, but later they'll vary with context (drop your weapon and you're punching not slashing).
                        nextActionSet = receivePlayerInput();
                    }
                    else
                    {
                        nextActionSet = Sys.Agent.GetCombatAction(battlefieldAgent.ActiveCombatAI, id, battlefieldEntityIds);
                    }


                }



                //var results = GettingStuff();

                //if (results.CombatFinished)
                //{
                //    yield return results.Strings();
                //    yield break;
                //}
                //else if (results == "needinput")
                //{
                //    string input = this.ReceivePlayerInputFunc();
                //    this.ApplyInput(input);
                //}
                //else
                //{
                //    yield return results.Strings();
                //}
            }
        }
    }
}
