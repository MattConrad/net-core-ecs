using System.Collections.Generic;

namespace SampleGame.Parts
{
    public class Faction : EntropyEcsCore.EcsEntityPart
    {
        //for now at least, agents will have a primary faction and a current faction in addition to their detailed faction reps.
        // this is attempting to make some things simpler later on.

        public static string PrimaryPublicFaction { get; set; } = "";
        public static string CurrentPublicFaction { get; set; } = "";

        /// <summary>
        /// A dictionary of factions and your reputation within each one, let's say from -100 to 100.
        /// Someday maybe we'll get fancier with different types of rep rather than just +/-.
        /// </summary>
        public Dictionary<string, int> PublicFactionReputations { get; set; } = new Dictionary<string, int>();
    }
}
