
namespace SampleGame.Parts
{
    public class Damager : EntropyEcsCore.EcsEntityPart
    {
        //these are what make weapons (or other entities) deal damage.
        // there might be a bunch of these on the same weapon (or spell, or whatever)

        //eg, single entity, battlefield, aoe, enemies only.
        public string TargetType { get; set; }

        /// <summary>
        /// Defined string value
        /// </summary>
        public string DamageType { get; set; }

        /// <summary>
        /// Usually a multiple of 1000: 1000, 2000, 3000 . . .
        /// </summary>
        public int DamageAmount { get; set; }
    }
}
