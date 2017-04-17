using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    //MWCTODO: probably, all this goes away and is replaced with enums used in CombatSystem. not crazy about messages any more.
    internal static class CombatActionMessage
    {
        internal static class Keys
        {
            /// <summary>
            /// List[string]
            /// </summary>
            public const string Results = nameof(Results);
            /// <summary>
            /// String: enum.
            /// </summary>
            public const string TargetType = nameof(TargetType);
            /// <summary>
            /// Long
            /// </summary>
            public const string TargetEntityId = nameof(TargetEntityId);
            /// <summary>
            /// Long
            /// </summary>
            public const string RawRoll = nameof(RawRoll);
            /// <summary>
            /// DataDict
            /// </summary>
            public const string RollModifiers = nameof(RollModifiers);
        }
    }

    internal static class CombatSystem
    {
        public static List<string> HerosAction(EcsRegistrar rgs, long heroId, long villianId, out bool combatFinished)
        {
            List<string> results = new List<string>();

            results.AddRange(ResolveAction(rgs, heroId, villianId, out combatFinished));

            if (!combatFinished) results.AddRange(ResolveAction(rgs, villianId, heroId, out combatFinished));

            return results;
        }

        private static List<string> ResolveAction(EcsRegistrar rgs, long attackerId, long targetId, out bool combatFinished)
        {
            combatFinished = false;
            var results = new List<string>();

            int roll = RandomSystem.GetRange5();
            results.Add($"Attacker rolled a {roll}.");

            ////maybe we'll optimize this later
            //var targetEquipment = ContainerSystem
            //    .GetEntityIdsFromFirstContainerByDesc(rgs, targetId, CpContainer.Vals.ContainerDescription.Equipped);

            ////for this simple game, there's only one piece of armor, but we'll use SelectMany anyhow.
            //var armor = targetEquipment.SelectMany(eid => rgs.GetComponentsOfType(eid, "MWCTODO:ARMOR")).FirstOrDefault();

            bool deathAndQuit = RandomSystem.Get1To100() > 99;
            if (deathAndQuit)
            {
                combatFinished = true;
                results.Add("Someone died. Combat ends.");
            }

            return new List<string>();
        }
    }
}
