using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    internal class EntityName : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// "Jack" or "Jack Aubrey" or "Excalibur".
        /// </summary>
        internal string ProperName { get; set; }
        /// <summary>
        /// "person" or "sword".
        /// </summary>
        internal string GeneralName { get; set; }
        /// <summary>
        /// "humanoid figure", or "long thin object"
        /// </summary>
        internal string ObscuredName { get; set; }
        /// <summary>
        /// Complete sentence. "A stout man with a cheerful, weathered face.", or "A sword of elegant make, in pristine condition."
        /// </summary>
        internal string ShortDescription { get; set; }
        /// <summary>
        /// A long description of a entity up to a hundred words or so. Not sure we actually want this.
        /// </summary>
        internal string LongDescription { get; set; }
        /// <summary>
        /// Defined string value. Used by the system for grammatical conversions ("he" yielding "him", "his", etc).
        /// </summary>
        internal string Pronoun { get; set; }

        internal static class Vals
        {
            internal static class Pronoun
            {
                public const string He = "he";
                public const string She = "she";
                public const string It = "it";
                public const string They = "they";
            }
        }
    }
}
