﻿
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
        /// A general multiplier altering the default damage of all otherwise-unspecified types. A standard issue human represents the baseline
        /// with a default damage multiplier of 1.0. The player character has extra survivability, and so has a multiplier of 0.66. 
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

        //MWCTODO: maybe this should be equippable size.
        /// <summary>
        /// Any object might be wielded as a weapon or a shield. Wieldable size tells us what size of being can wield/equip the object one-handed.
        /// The equipment system has default rules on how deviations from this size will work.
        /// </summary>
        public string WieldableSize { get; set; }

        /// <summary>
        /// For explicitly equippable items, in what (default humanoid) slot the item is equippable/wieldable.
        /// This couples Parts.PhysicalObject to Parts.Anatomy, which I don't like, but don't have a better idea about right now.
        /// </summary>
        public string EquippableSlot { get; set; }
    }
}
