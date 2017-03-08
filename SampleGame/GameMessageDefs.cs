using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleGame.MessageDefs
{
    public static class AlterContainerContentsResultsMessage
    {
        public static class Keys
        {
            /// <summary>
            /// Bool.
            /// </summary>
            public const string Succeeded = nameof(Succeeded);

            /// <summary>
            /// List[string]. Unless this needs to be a DataDict.
            /// </summary>
            public const string Output = nameof(Output);
        }

    }

    public static class AttackMessage
    {
        public static class Keys
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

        public static class Vals
        {
            public static class TargetType
            {
                public const string SingleMelee = nameof(SingleMelee);
                public const string SingleRanged = nameof(SingleRanged);
                public const string Aoe = nameof(Aoe);
            }

            public static class MeleeWeaponType
            {
                public const string BareFist = nameof(BareFist);
                public const string Dagger = nameof(Dagger);
                public const string LongSword = nameof(LongSword);
                public const string Claymore = nameof(Claymore);
            }

            public static class DamageType
            {
                public const string Mechanical = nameof(Mechanical);
                public const string Heat = nameof(Heat);
                public const string Electric = nameof(Electric);
                public const string Poison = nameof(Poison);
            }
        }

    }

    public static class ConsoleMessage
    {
        public static class Keys
        {
            /// <summary>
            /// ListString: lines to write to output.
            /// </summary>
            public const string Lines = nameof(Lines);
        }
    }

}
