using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Sys
{
    internal static class PhysicalObject
    {
        //i *think* this belongs here. short term stuff anyway. MWCTODO: maybe it should be part of the part, instead.
        internal static string GetLivingThingConditionDesc(long condition)
        {
            if (condition >= 10000) return "untouched";
            if (condition >= 8000) return "bruised and scratched";
            if (condition >= 6000) return "battered and bleeding";
            if (condition >= 4000) return "injured but determined";
            if (condition >= 2000) return "looking fearful";
            if (condition >= 1) return "in desperate straits";

            return "dead";
        }
    }
}
