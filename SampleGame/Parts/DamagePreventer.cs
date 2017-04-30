
namespace SampleGame.Parts
{
    public class DamagePreventer : EntropyEcsCore.EcsEntityPart
    {
        //MWCTODO: eventually, each damage preventer type should have a specific damage type that it prevents.
        // a piece of armor might be composed of multiple damage preventers.

        /// <summary>
        /// A general damage threshold, below which all damage is absorbed, and after which the damage multiplier is applied.
        /// The baseline is 0, and (for now) a DDT of 10000 is complete invincibility. It's OK for most things to have a DDT of 0. 
        /// Something immune to weak attacks might have a DDT of 1000. A diamond wall might have a DDT of 8000 (and a low multiplier too).
        /// </summary>
        public int DamageThreshold { get; set; } = 0;
        /// <summary>
        /// A general multiplier altering the default damage of all otherwise-unspecified types. The player character represents the baseline
        /// with a default damage multiplier of 1.0. An undistinguished NPC maybe 2.0 (the player character has extra survivability). 
        /// Something small and frail like a housecat might be a 6 or 7 (but with good evasion). A hill giant might have something like 0.3.
        /// A heavy stone door would have something like 0.01.
        /// </summary>
        public decimal DamageMultiplier { get; set; } = 1.0m;

        //MWCTODO: later, ofc, we need lots of damage types and specific protections here.
    }
}
