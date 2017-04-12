using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    internal static class CpEntityName
    {
        internal static class Keys
        {
            //MWCTODO: ugh. these aren't clear enough and in any case each needs an illustrating example in XML header comments.
            public const string ProperName = nameof(ProperName);
            public const string VagueName = nameof(VagueName);
            public const string ShortDescription = nameof(ShortDescription);
            public const string LongDescription = nameof(LongDescription);
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
