using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Skills
    {
        internal static Dictionary<string, int> GetAdjustedSkills(EcsRegistrar rgs, long entityId )
        {
            var skillDicts = rgs.GetPartsOfType<Parts.Skillset>(entityId).Select(p => p.Skills);
            var modifierDicts = rgs.GetPartsOfType<Parts.SkillsModifier>(entityId).Select(p => p.SkillDeltas);

            return skillDicts.Union(modifierDicts)
                .SelectMany(d => d)
                .ToLookup(dicts => dicts.Key, dicts => dicts.Value)
                .ToDictionary(l => l.Key, l => l.Sum());
        }

        internal static Dictionary<string, int> GetAdjustedSkills(EcsRegistrar rgs, IEnumerable<long> entityIds)
        {
            return entityIds
                .Select(id => GetAdjustedSkills(rgs, id))
                .SelectMany(d => d).ToLookup(dicts => dicts.Key, dicts => dicts.Value)
                .ToDictionary(l => l.Key, l => l.Sum());
        }

    }
}
