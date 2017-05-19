using System.Collections.Generic;

namespace SampleGame.Parts
{
    public class Faction : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// A dictionary of factions and your reputation within each one, let's say from -100 to 100.
        /// Someday maybe we'll get fancier with different types of rep rather than just +/-.
        /// </summary>
        public Dictionary<string, int> FactionReputations { get; set; } = new Dictionary<string, int>();
    }
}
