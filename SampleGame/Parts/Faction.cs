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

        //I do like this approach. I think. might could use code generation to implement these as partial classes.
        public static class Vals
        {
            public static class FactionName
            {
                public const string Heroes = nameof(Heroes);
                public const string Villians = nameof(Villians);
            }
        }
    }
}
