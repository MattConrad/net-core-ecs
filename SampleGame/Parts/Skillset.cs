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
            //"attributes" and skills have the same structure.
            public static class Attribute
            {
                public const string Physical = nameof(Physical);
                public const string Magical = nameof(Magical);
            }

            public static class Physical
            {
                public const string Melee = nameof(Melee);
                public const string Dodge = nameof(Dodge);
            }
        }
    }
}
