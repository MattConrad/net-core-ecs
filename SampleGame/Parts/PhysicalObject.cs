
namespace SampleGame.Parts
{
    public class PhysicalObject : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// Some world objects will be incorporeal, and have special or absent behavior compared to normal objects.
        /// </summary>
        public bool Corporeal { get; set; } = true;
        /// <summary>
        /// Some things can't be moved without destroying them.
        /// </summary>
        public bool Moveable { get; set; } = true;
        /// <summary>
        /// Range 0-10000 with 0 = destroyed and 10000 = perfect health. For living things, these are hit points.
        /// You can divide by 100 to get the % condition of any physical object.
        /// Notice that a housecat and a hill giant both start with 10000 hit points--but see DefaultDamageMultiplier. 
        /// </summary>
        public int Condition { get; set; } = 10000;

        //i have waffled about whether objects, or only damage preventers, should have damage threshold and multiplier.
        //  presently i think if it has a condition, it's reasonable to have a default for these without necessarily having a damagepreventer part.

        //eventually there will be damage-specific thresholds and multipliers, probably a dict with a key for each different damage type. not yet though.
        /// <summary>
        /// A general damage threshold, below which all damage is absorbed, and after which the damage multiplier is applied.
        /// The baseline is 0, and (for now) a DDT of 10000 is complete invincibility. It's OK for most things to have a DDT of 0. 
        /// Something immune to weak attacks might have a DDT of 1000. A diamond wall might have a DDT of 8000 (and a low multiplier too).
        /// </summary>
        public int DefaultDamageThreshold { get; set; } = 0;

        /// <summary>
        /// A general multiplier altering the default damage of all otherwise-unspecified types. The player character represents the baseline
        /// with a default damage multiplier of 1.0. An undistinguished NPC maybe 2.0 (the player character has extra survivability). 
        /// Something small and frail like a housecat might be a 6 or 7 (but with good evasion). A hill giant might have something like 0.3.
        /// A heavy stone door would have something like 0.01.
        /// </summary>
        public decimal DefaultDamageMultiplier { get; set; } = 1.0m;
        
        /// <summary>
        /// Weight in pounds.
        /// </summary>
        public decimal Weight { get; set; }
        
        /// <summary>
        /// Defined string value.
        /// </summary>
        public string Size { get; set; }

        //wieldabliity is something we probably do want here, but I haven't figured it out yet. maybe a long enum with all the size permuations...nah. probably a list of allowed sizes.

        //can we spec the universe with only seven sizes? maybe.
        public static class Vals
        {
            public static class Size
            {
                //hmmm, is this a good way to handle? if you keep this, each of these categories needs an example. (or examples)
                public const string Minuscule = "minuscule";
                public const string Tiny = "tiny";
                public const string Small = "small";
                public const string Medium = "medium";
                public const string Big = "big";
                public const string VeryBig = "very big";
                public const string Enormous = "enormous";
            }
        }
    }
}
