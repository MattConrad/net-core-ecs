﻿
namespace SampleGame.Parts
{
    public class SingleTargetDamager : EntropyEcsCore.EcsEntityPart
    {
        //these are what make weapons (or other entities) deal damage.
        // there might be a bunch of these on the same weapon (or spell, or whatever)

        /// <summary>
        /// Defined string value
        /// </summary>
        public string DamageType { get; set; }

        /// <summary>
        /// Usually a multiple of 1000: 1000, 2000, 3000 . . .
        /// </summary>
        public int DamageAmount { get; set; }

        public static class Vals
        {
            public static class DamageType
            {
                public const string MechanicalPiercing = nameof(MechanicalPiercing);
                public const string MechanicalSlashing = nameof(MechanicalSlashing);
            }
        }
    }
}
