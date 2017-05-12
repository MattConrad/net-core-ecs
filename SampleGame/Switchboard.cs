//using System;
//using System.Collections.Generic;
//using System.Text;

//probably this goes away.

//namespace SampleGame
//{
//    public static class Switchboard
//    {
//        //eventually, this should be contextual for the hero at any given point in time.
//        //  well, the hero and any npcs that the player might be controlling
//        //  also, it would be nice if these were nested, so that Change Stance would 
//        //  subordinate the different stances.
//        public static Dictionary<string, string> HerosActions()
//        {
//            return new Dictionary<string, string>
//            {
//                ["Attack (Melee)"] = Sys.Combat.Actions.AttackMelee,
//                ["Attack Continously (for testing)"] = Sys.Combat.Actions.AttackMeleeContinously,
//                ["Stance (Defensive)"] = Sys.Combat.Actions.StanceDefensive,  
//                ["Stance (Stand Ground)"] = Sys.Combat.Actions.StanceStandGround,  
//                ["Stance (Aggressive)"] = Sys.Combat.Actions.StanceAggressive
//            };
//        }
//    }
//}
