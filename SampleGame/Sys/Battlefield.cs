using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Battlefield
    {
        internal static IEnumerable<List<string>> RunBattlefield(EcsRegistrar rgs, long battlefieldId)
        {
            while (true)
            {
                var battlefieldEntityIds = rgs.GetPartsOfType<Parts.Container>(battlefieldId)
                    .Single(p => p.Tag == Parts.Container.Vals.Tag.Battlefield)
                    .EntityIds;

                foreach(long entityId in battlefieldEntityIds)
                {

                }

                //eventually, entities may enter or leave the battlefield, so let's recheck every time.

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
