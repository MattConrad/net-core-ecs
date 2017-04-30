using System.Collections.Generic;

namespace SampleGame.Parts
{
    public class Skillset : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// A dictionary of skill names and a value for each skill. 
        /// Skills are ints, usually but not always ranging from 1 - 5.
        /// Eventually, skills will relate to attributes, but not yet.
        /// </summary>
        public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

        public static class Vals
        {
            public static class Physical
            {
                public const string Melee = nameof(Melee);
            }
        }
    }
}
