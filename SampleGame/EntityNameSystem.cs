using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    internal static class CpEntityName
    {
        internal static class Keys
        {
            /// <summary>
            /// String. "Jack" or "Jack Aubrey" or "Excalibur".
            /// </summary>
            public const string ProperName = nameof(ProperName);
            /// <summary>
            /// String. "person" or "sword".
            /// </summary>
            public const string GeneralName = nameof(GeneralName);
            /// <summary>
            /// String. "humanoid figure", or "long object like a pole"
            /// </summary>
            public const string ObscuredName = nameof(ObscuredName);
            /// <summary>
            /// String. Complete sentence. "A stout man with a cheerful, weathered face.", or "A sword of elegant make, in pristine condition."
            /// </summary>
            public const string ShortDescription = nameof(ShortDescription);
            /// <summary>
            /// String. A long description of a entity up to a hundred words or so.
            /// </summary>
            public const string LongDescription = nameof(LongDescription);
            /// <summary>
            /// String: enum. Used by the system for grammatical conversions ("he" yielding "him", "his", etc).
            /// </summary>
            public const string Pronoun = nameof(Pronoun);
        }

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

        /// <summary>
        /// Create an un-ided component.
        /// </summary>
        internal static EcsComponent Create(string properName = "", string generalName = "", string obscuredName = "",
            string shortDescription = "", string longDescription = "", string pronoun = Vals.Pronoun.It)
        {
            var cp = new EcsComponent { Type = nameof(CpEntityName), Data = new DataDict() };

            cp.Data[Keys.ProperName] = properName ?? "";
            cp.Data[Keys.GeneralName] = generalName ?? "";
            cp.Data[Keys.ObscuredName] = obscuredName ?? "";
            cp.Data[Keys.ShortDescription] = shortDescription ?? "";
            cp.Data[Keys.LongDescription] = longDescription ?? "";
            cp.Data[Keys.Pronoun] = pronoun;

            return cp;
        }
    }

    internal static class EntityNameSystem
    {
        internal static DataDict GetEntityNames(EcsRegistrar rgs, long entityId)
        {
            return rgs.GetComponentsOfType(entityId, nameof(CpEntityName)).First().Data;
        }
    }
}
