using System.Collections.Generic;

namespace SampleGame.Parts
{
    internal class Faction : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// A dictionary of factions and your reputation within each one, let's say from -100 to 100.
        /// Someday maybe we'll get fancier with different types of rep rather than just +/-.
        /// </summary>
        internal Dictionary<string, int> FactionReputations { get; set; } = new Dictionary<string, int>();

        //I do like this approach. I think. might could use code generation to implement these as partial classes.
        internal static class Vals
        {
            internal static class FactionName
            {
                internal const string Heroes = nameof(Heroes);
                internal const string Villians = nameof(Villians);
            }
        }
    }
}
