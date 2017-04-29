using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    internal class Skillset : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// A dictionary of skill names and a value for each skill. 
        /// Skills are ints, usually but not always ranging from 1 - 5.
        /// Eventually, skills will relate to attributes, but not yet.
        /// </summary>
        internal Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

        internal static class Vals
        {
            internal static class Physical
            {
                internal const string Melee = nameof(Melee);
            }
        }
    }
}
