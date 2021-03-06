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

        /// <summary>
        /// Which slots can this item be equipped to? Usually, either of two hands, either of two ring fingers, head, etc.
        /// </summary>
        public Vals.BodySlots EquipmentSlots { get; set; }

        /// <summary>
        /// If the object is equippable, what slot does it occupy? Most items aren't equippable and so this will be null.
        /// </summary>
        public string EquipmentSlotName { get; set; }

        /// <summary>
        /// If the object is equippable, but it is special in what slots it occupies, this gives a code referring to detailed logic about its specialness.
        /// This will almost always be null.
        /// </summary>
        public string EquipmentSpecialCode { get; set; }

        /// <summary>
        /// Verb for the melee action: "slash", "bite", "stab". Considering something like "+slash" which can be unfolded to "slash" or "hack" or "cut" or "slice".
        /// </summary>
        public string MeleeVerb { get; set; } = "bash";

        /// <summary>
        /// Verb for the ranged action: "fire", "launch", "shoot". Likewise possibly expandable values.
        /// </summary>
        public string RangedVerb { get; set; } = "throw";
    }
}
