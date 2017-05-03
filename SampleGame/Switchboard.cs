using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public static class Switchboard
    {
        //eventually, this should be contextual for the hero at any given point in time.
        //  well, the hero and any npcs that the player might be controlling
        //  also, it would be nice if these were nested, so that Change Stance would 
        //  subordinate the different stances.
        public static Dictionary<string, string> HerosActions()
        {
            return new Dictionary<string, string>
            {
                ["Attack (Melee)"] = "attack-melee",                  ["Stance (Defensive)"] = "stance-defensive",                  ["Stance (Stand Ground)"] = "stance-stand-ground",  
                ["Stance (Aggressive)"] = "stance-aggressive"
            };
        }
    }
}
