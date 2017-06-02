using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public static class Switchboard
    {


    }

    internal class PossibleCombatActions
    {
        internal List<string> SingleTargetMeleeAttacks { get; set; } = new List<string>();
        internal Dictionary<long, string> MeleeTargets { get; set; } = new Dictionary<long, string>();
        internal List<string> SingleTargetRangedAttacks { get; set; } = new List<string>();
        internal Dictionary<long, string> RangedTargets { get; set; } = new Dictionary<long, string>();
    }

}
