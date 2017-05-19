using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public static class CombatActions
    {
        public const string DoNothing = "do-nothing";
        public const string AttackMelee = "attack-melee";
        public const string SwitchToAI = "switch-to-ai";
        public const string StanceDefensive = "stance-defensive";
        public const string StanceStandGround = "stance-stand-ground";
        public const string StanceAggressive = "stance-aggressive";
        public const string Dodge = "dodge";
    }

    public static class DamageType
    {
        public const string MechanicalPiercing = nameof(MechanicalPiercing);
        public const string MechanicalSlashing = nameof(MechanicalSlashing);
    }

}
