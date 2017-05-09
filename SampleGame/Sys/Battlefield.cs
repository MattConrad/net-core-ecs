using System;
using System.Collections.Generic;
using System.Text;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    public static class Battlefield
    {
        public static List<string> RunBattlefield(EcsRegistrar rgs, long battlefieldId)
        {
            //MWCTODO: not sure if this really returns List<string> or not. maybe return IEnumerable<List<string>> w/ yield. 
            // i think this is a battefield loop that runs forever until done.
            // not sure how it pauses for input or returns results.
            return null;
        }
    }
}
