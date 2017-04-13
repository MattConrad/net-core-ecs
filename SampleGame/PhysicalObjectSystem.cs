using System.Collections.Generic;
using EntropyEcsCore;

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
            /// Decimal: range 0-1 with 0 = destroyed and 1 = perfect condition.
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

            ///// <summary>
            ///// String: enum
            ///// </summary>
            //public const string WieldableModifier = nameof(WieldableModifier);
        }

        internal static class Vals
        {
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

            //internal static class DamageType
            //{
            //    public const string Mechanical = nameof(Mechanical);
            //    public const string Heat = nameof(Heat);
            //    public const string Electric = nameof(Electric);
            //    public const string Poison = nameof(Poison);
            //}
        }

        /// <summary>
        /// Create an un-ided component.
        /// </summary>
        internal static EcsComponent Create(bool corporeal = true, decimal condition = 1.0M, decimal weight = 0M, string size = Vals.Size.Medium)
        {
            var cp = new EcsComponent { Type = nameof(CpPhysicalObject), Data = new DataDict() };

            cp.Data[Keys.Corporeal] = corporeal;
            cp.Data[Keys.Condition] = condition;
            cp.Data[Keys.Weight] = weight;
            cp.Data[Keys.Size] = size;

            return cp;
        }
    }
}
