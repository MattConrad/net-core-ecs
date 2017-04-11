using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    /// <summary>
    /// This component is always present on entities that are in-game objects.
    /// It provides default behaviors for physical objects (and perhaps sounds, golden glows, etc).
    /// Most objects will include other components for more specific behavior.
    /// </summary>
    internal static class CpPhysicalObject
    {
        internal static class Keys
        {
            /// <summary>
            /// Bool. Some world objects will be incorporeal, and have special or absent behavior compared to normal objects.
            /// </summary>
            public const string Corporeal = nameof(Corporeal);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string Condition = nameof(Condition);

            /// <summary>
            /// Decimal, weight in pounds.
            /// </summary>
            public const string Weight = nameof(Weight);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string Size = nameof(Size);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string SizeModifier = nameof(SizeModifier);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string CarryableModifier = nameof(CarryableModifier);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string WieldableModifier = nameof(WieldableModifier);
        }

        internal static class Vals
        {
            internal static class Condition
            {
                public const string Destroyed = nameof(Destroyed);
                public const string NearlyDestroyed = nameof(NearlyDestroyed);
                public const string Damaged = nameof(Damaged);
                public const string SlightlyDamaged = nameof(SlightlyDamaged);
                public const string Undamaged = nameof(Undamaged);
            }

            internal static class Size
            {
                //hmmm, is this a good way to handle?
                public const string Miniscule = nameof(Miniscule);
                public const string Tiny = nameof(Tiny);
                public const string Small = nameof(Small);
                public const string Medium = nameof(Medium);
                public const string Big = nameof(Big);
                public const string VeryBig = nameof(VeryBig);
                public const string Enormous = nameof(Enormous);
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
}
