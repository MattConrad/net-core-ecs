using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    internal static class CombatActionMessage
    {
        internal static class Keys
        {
            /// <summary>
            /// Bool
            /// </summary>
            public const string Continuing = nameof(Continuing);
            /// <summary>
            /// List[string]
            /// </summary>
            public const string Results = nameof(Results);
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
        //MWCTOOD: and now I begin to doubt that we want to pass dictionaries everywhere.
        // maybe we don't want to use dicts for messages, only for components.
        // if this is true, does it affect DataDict? probably not, but maybe.
        //MWCTODO: this is only temporary. real endpoints need to be a lot more flexible than this.
        public static List<string> AttemptAction(EcsRegistrar rgs, long attackerId, long targetId, out bool continuing)
        {
            var msg = new DataDict();
            msg.Add(CombatActionMessage.Keys.TargetType, CombatActionMessage.Vals.TargetType.SingleMelee);
            msg.Add(CombatActionMessage.Keys.TargetEntityId, targetId);

            var results = ResolveAction(rgs, attackerId, msg);

            continuing = results.GetBool(CombatActionMessage.Keys.Continuing);
            return msg.GetListString(CombatActionMessage.Keys.Results);
        }

        private static DataDict ResolveAction(EcsRegistrar rgs, long attackerId, DataDict attackData)
        {
            var results = new List<string>();

            var targetType = attackData.GetString(CombatActionMessage.Keys.TargetType);
            if (targetType != CombatActionMessage.Vals.TargetType.SingleMelee) throw new ArgumentException("Melee only.");

            int roll = RandomSystem.GetRange5();
            results.Add($"Attacker rolled a {roll}.");

            long targetId = attackData.GetLong(CombatActionMessage.Keys.TargetEntityId);
            //maybe we'll optimize this later
            var targetEquipment = ContainerSystem
                .GetEntityIdsFromFirstContainer(rgs, targetId, CpContainer.Vals.ContainerDescription.Equipped);

            ////for this simple game, there's only one piece of armor, but we'll use SelectMany anyhow.
            //var armor = targetEquipment.SelectMany(eid => rgs.GetComponentsOfType(eid, "MWCTODO:ARMOR")).First();

            bool deathAndQuit = RandomSystem.GetRange5() > 3;
            if (deathAndQuit) results.Add("Someone died. Combat ends.");

            attackData.Add(CombatActionMessage.Keys.Results, results);
            attackData.Add(CombatActionMessage.Keys.Continuing, !deathAndQuit);

            return attackData;
        }
    }
}
