using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public static class Switchboard
    {
        //i could see this becoming a separate project eventually.
        // if so, actor choice categories would need to move here from vals.
    }

    internal class PossibleCombatActions
    {
        public List<ActorChoicesPossible> NonAttackActions { get; set; } = new List<ActorChoicesPossible>();
        public List<ActorChoicesPossible> SingleTargetMeleeAttacks { get; set; } = new List<ActorChoicesPossible>();
        public Dictionary<long, string> MeleeTargets { get; set; } = new Dictionary<long, string>();
        public List<ActorChoicesPossible> SingleTargetRangedAttacks { get; set; } = new List<ActorChoicesPossible>();
        public Dictionary<long, string> RangedTargets { get; set; } = new Dictionary<long, string>();
    }

    internal class ActorChoicesPossible
    {
        public string Category { get; set; }
        public string Action { get; set; }
        //weapon name or "punch" for now.
        public string ActionWeaponOrSpellName { get; set; }
        //for now, this is just main hand/offhand
        public string ActionModifier { get; set; }
        public int? WeaponHandIndex { get; set; }
        public long? WeaponEntityId { get; set; }
        public string NextCategory { get; set; }
    }

    public class ActionChosen
    {
        public string Action { get; set; }
        public int? WeaponHandIndex { get; set; }
        public long? WeaponEntityId { get; set; }
        public int? TargetEntityId { get; set; }
    }


}
