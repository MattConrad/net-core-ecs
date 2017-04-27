
namespace SampleGame.Parts
{
    internal class SingleTargetDamager : EntropyEcsCore.EcsEntityPart
    {
        //these are what make weapons (or other entities) deal damage.
        // there might be a bunch of these on the same weapon (or spell, or whatever)

        /// <summary>
        /// Defined string value
        /// </summary>
        internal string DamageType { get; set; }

        /// <summary>
        /// Usually a multiple of 1000: 1000, 2000, 3000 . . .
        /// </summary>
        internal int DamageAmount { get; set; }

        internal static class Vals
        {
            internal static class DamageType
            {
                internal const string MechanicalPiercing = nameof(MechanicalPiercing);
                internal const string MechanicalSlashing = nameof(MechanicalSlashing);
            }
        }
    }
}
