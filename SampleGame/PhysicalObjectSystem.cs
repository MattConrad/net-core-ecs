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
            /// Long: range 0-10000 with 0 = destroyed and 10000 = perfect health. For living things, these are hit points.
            /// You can divide by 100 to get the % condition of any physical object.
            /// Notice that a housecat and a hill giant both start with 10000 hit points--but see DefaultDamageMultiplier. 
            /// </summary>
            public const string Condition = nameof(Condition);

            //MWCTODO: condition desc? "health", "condition", "working order"?
            
            //MWCTODO: i dunno, this may be too different from other systems to work. but let's try it. the idea of a consistent scale for condition appeals.
            //MWCTODO: do we want damage threshold AND damage multiplier? cause if we want both you need to think about how they'll work together
            //     for example, which one is applied first? does it even make sense to have a common value for incoming damage with both DT and DM?
            /// <summary>
            /// Decimal: A general multiplier altering the default damage of all otherwise-unspecified types. A person in good health has
            /// a default damage multiplier of 1.0. A housecat would have something like 5.0 (but also good evasion). A hill giant would have something like 0.2.
            /// A heavy stone door would have something like 0.005.
            /// (eventually) There are damage type specific modifiers also, which will take precedence over the default for that damage type.
            /// </summary>
            public const string DefaultDamageMultiplier = nameof(DefaultDamageMultiplier);

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
                public const string Minuscule = "minuscule";
                public const string Tiny = "tiny";
                public const string Small = "small";
                public const string Medium = "medium";
                public const string Big = "big";
                public const string VeryBig = "very big";
                public const string Enormous = "enormous";
            }

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
