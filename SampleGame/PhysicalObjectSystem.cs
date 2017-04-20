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

            // Moveable?

            //MWCTODO: i dunno, this may be too different from other systems to work. but let's try it. the idea of a consistent scale for condition appeals.
            /// <summary>
            /// Long: range 0-10000 with 0 = destroyed and 10000 = perfect health. For living things, these are hit points.
            /// You can divide by 100 to get the % condition of any physical object.
            /// Notice that a housecat and a hill giant both start with 10000 hit points--but see DefaultDamageMultiplier. 
            /// </summary>
            public const string Condition = nameof(Condition);

            //MWCTODO: condition desc? "health", "condition", "working order"?

            //eventually there will be damage-specific thresholds and multipliers, probably a dict with a key for each different damage type. not yet though.

            /// <summary>
            /// Decimal: A general damage threshold, below which all damage is absorbed, and after which the damage multiplier is applied.
            /// The baseline is 0, and (for now) a DDT of 10000 is complete invincibility. It's OK for most things to have a DDT of 0. 
            /// Something immune to weak attacks might have a DDT of 1000. A diamond wall might have a DDT of 8000 (and a low multiplier too).
            /// </summary>
            public const string DefaultDamageThreshold = nameof(DefaultDamageThreshold);

            //MWCTODO: maybe this should be divisor instead of multiplier. what is least confusing in conjunction with damage threshold?
            /// <summary>
            /// Decimal: A general multiplier altering the default damage of all otherwise-unspecified types. The player character represents the baseline
            /// with a default damage multiplier of 1.0. An undistinguished NPC maybe 2.0 (the player character has extra survivability). 
            /// Something small and frail like a housecat might be a 6 or 7 (but with good evasion). A hill giant might have something like 0.3.
            /// A heavy stone door would have something like 0.01.
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

            //most things which are moveable should also be wieldable as melee weapons and throwable as missile weapons
            //  should weapons be a separate component along with physical object, or just attributes that any physical object might have? i'm going back and forth here . . . figure it out later.
            // I'd like these to have penalties based on size and weight, relative to size and strength of the wielder.
            // So an oak chair is a workable weapon for a big strong guy and not so much for a little weak one.
            // Might be going down a rabbit hole here, maybe should keep it simpler in the beginning. 
            /// <summary>
            /// String: enum
            /// </summary>
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

        // MWCTODO: This is just temporary for testing, doesn't belong here, maybe on a static PhysicalObjectSystem. replace with something better and less tacked-on.
        internal static string GetLivingThingConditionDesc(long condition)
        {
            if (condition >= 10000) return "untouched";
            if (condition >= 8000) return "bruised and scratched";
            if (condition >= 6000) return "battered and bleeding";
            if (condition >= 4000) return "injured but determined";
            if (condition >= 2000) return "looking fearful";
            if (condition >= 1) return "in desperate straits";

            return "dead";
        }

        /// <summary>
        /// Create an un-ided component.
        /// </summary>
        internal static EcsComponent Create(bool corporeal = true, long condition = 10000L, decimal weight = 0M, string size = Vals.Size.Medium)
        {
            var cp = new EcsComponent { Type = nameof(CpPhysicalObject), Data = new DataDict() };

            cp.Data[Keys.Corporeal] = corporeal;
            cp.Data[Keys.Condition] = condition;
            cp.Data[Keys.Weight] = weight;
            cp.Data[Keys.Size] = size;
            //MWCTODO: add params for these, and set the damage mulitplier to something orcish on the orc and heroish on the hero.
            cp.Data[Keys.DefaultDamageThreshold] = 0L;
            cp.Data[Keys.DefaultDamageMultiplier] = 1.0m;

            return cp;
        }
    }
}
