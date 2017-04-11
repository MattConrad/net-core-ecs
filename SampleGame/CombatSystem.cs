using System;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    internal static class AttackMessage
    {
        internal static class Keys
        {
            /// <summary>
            /// String: enum.
            /// </summary>
            public const string TargetType = nameof(TargetType);
            /// <summary>
            /// Long
            /// </summary>
            public const string TargetEntityId = nameof(TargetEntityId);
            /// <summary>
            /// Long
            /// </summary>
            public const string RawRoll = nameof(RawRoll);
            /// <summary>
            /// DataDict
            /// </summary>
            public const string RollModifiers = nameof(RollModifiers);
            /// <summary>
            /// String: enum
            /// </summary>
            public const string DamageType = nameof(DamageType);
        }

        internal static class Vals
        {
            internal static class TargetType
            {
                public const string SingleMelee = nameof(SingleMelee);
                public const string SingleRanged = nameof(SingleRanged);
                public const string Aoe = nameof(Aoe);
            }

            internal static class MeleeWeaponType
            {
                public const string BareFist = nameof(BareFist);
                public const string Dagger = nameof(Dagger);
                public const string LongSword = nameof(LongSword);
                public const string Claymore = nameof(Claymore);
            }

            internal static class DamageType
            {
                public const string Mechanical = nameof(Mechanical);
                public const string Heat = nameof(Heat);
                public const string Electric = nameof(Electric);
                public const string Poison = nameof(Poison);
            }
        }
    }

    internal static class CombatSystem
    {
        internal static DataDict ResolveAttack(EcsRegistrar rgs, long attackerId, DataDict attackData)
        {
            var targetType = attackData.GetString(AttackMessage.Keys.TargetType);
            if (targetType != AttackMessage.Vals.TargetType.SingleMelee) throw new ArgumentException("Melee only.");

            int roll = RandomSystem.GetRange5();

            long targetId = attackData.GetLong(AttackMessage.Keys.TargetEntityId);
            //maybe we'll optimize this later
            var targetEquipment = ContainerSystem
                .GetEntityIdsFromFirstContainer(rgs, targetId, CpContainer.Vals.ContainerDescription.Equipped);

            //for this simple game, there's only one piece of armor, but we'll use SelectMany anyhow.
            var armor = targetEquipment.SelectMany(eid => rgs.GetComponentsOfType(eid, "MWCTODO:ARMOR")).First();

            return attackData;
        }
    }
}
