using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public static class CombatCategory
    {
        public const string AllMelee = nameof(AllMelee);
        public const string AllRanged = nameof(AllRanged);
        public const string AllSpells = nameof(AllSpells);
        public const string AllStances = nameof(AllStances);
        public const string SpecificMelee = nameof(SpecificMelee);
        public const string SpecificRanged = nameof(SpecificRanged);
        public const string SpecificSpell = nameof(SpecificSpell);
        public const string SpecificStance = nameof(SpecificStance);
        public const string MeleeTarget = nameof(MeleeTarget);
        public const string RangedTarget = nameof(RangedTarget);
    }


    public static class CombatAction
    {
        public static readonly string[] AllStances = new string[] { StanceDefensive, StanceStandGround, StanceAggressive };

        public const string DoNothing = nameof(DoNothing);
        public const string AttackWeaponMelee = nameof(AttackWeaponMelee);
        public const string AttackWeaponRanged = nameof(AttackWeaponRanged);
        public const string SwitchToAI = nameof(SwitchToAI);
        public const string StanceDefensive = nameof(StanceDefensive);
        public const string StanceStandGround = nameof(StanceStandGround);
        public const string StanceAggressive = nameof(StanceAggressive);
        public const string Dodge = nameof(Dodge);
    }

    public static class CombatStatusTag
    {
        public static readonly string[] CombatTerminalStatuses = new string[] { Dead };

        public const string Dead = nameof(Dead);
    }

    public static class DamageType
    {
        public const string MechanicalBlunt = nameof(MechanicalBlunt);
        public const string MechanicalPiercing = nameof(MechanicalPiercing);
        public const string MechanicalSlashing = nameof(MechanicalSlashing);
    }

}
