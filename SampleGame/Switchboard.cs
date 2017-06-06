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
        public List<ActorChoiceSet> NonMeleeActions { get; set; } = new List<ActorChoiceSet>();
        public List<ActorChoiceSet> SingleTargetMeleeAttacks { get; set; } = new List<ActorChoiceSet>();
        public Dictionary<long, string> MeleeTargets { get; set; } = new Dictionary<long, string>();
        public List<ActorChoiceSet> SingleTargetRangedAttacks { get; set; } = new List<ActorChoiceSet>();
        public Dictionary<long, string> RangedTargets { get; set; } = new Dictionary<long, string>();
    }

    internal class ActorChoiceSet
    {
        //weapon name or "punch" for now.
        public string ActionWeaponOrSpellName { get; set; }
        //for now, this is just main hand/offhand
        public string ActionModifier { get; set; }
        public string Action { get; set; }
        public int? WeaponHandIndex { get; set; }
        public long DirectObjectEntityId { get; set; }
    }

}
