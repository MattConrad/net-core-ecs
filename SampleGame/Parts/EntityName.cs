
namespace SampleGame.Parts
{
    public class EntityName : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// "Jack" or "Jack Aubrey" or "Excalibur".
        /// </summary>
        public string ProperName { get; set; }
        /// <summary>
        /// "person" or "sword".
        /// </summary>
        public string GeneralName { get; set; }
        /// <summary>
        /// "humanoid figure", or "long thin object"
        /// </summary>
        public string ObscuredName { get; set; }
        /// <summary>
        /// Complete sentence. "A stout man with a cheerful, weathered face.", or "A sword of elegant make, in pristine condition."
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// A long description of a entity up to a hundred words or so. Not sure we actually want this.
        /// </summary>
        public string LongDescription { get; set; }
        /// <summary>
        /// Defined string value. Used by the system for grammatical conversions ("he" yielding "him", "his", etc).
        /// </summary>
        public string Pronoun { get; set; }

        public static class Vals
        {
            public static class Pronoun
            {
                public const string He = "he";
                public const string She = "she";
                public const string It = "it";
                public const string They = "they";
                public const string You = "you";
            }
        }
    }
}
