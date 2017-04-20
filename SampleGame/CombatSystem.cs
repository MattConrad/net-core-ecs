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

            int attackRoll = RandomSystem.GetRange5();
            results.Add($"Attacker rolled a {attackRoll}.");

            int defenseRoll = RandomSystem.GetRange5();
            results.Add($"Defender rolled a {defenseRoll}.");

            //eventually we'll want to watch for crits and fumbles
            //there will be a variety of modifiers, temp and perm, that can apply here.
            var modifiedDefenseRoll = Math.Max(defenseRoll, 0);
            var netAttack = Math.Max(attackRoll - modifiedDefenseRoll, 0);

            var targetPhysicalObject = rgs.GetComponentsOfType(targetId, nameof(CpPhysicalObject)).Single();

            //MWCTODO: probably all these key accesses ought to be hidden behind methods.
            //for now, we'll just multiply base roll by 1000, so doing 0-10 points damage in a turn, modified.
            var damageDealt = ((netAttack * 1000) 
                - targetPhysicalObject.Data.GetLong(CpPhysicalObject.Keys.DefaultDamageThreshold)) 
                * targetPhysicalObject.Data.GetDecimal(CpPhysicalObject.Keys.DefaultDamageMultiplier);

            targetPhysicalObject.Data[CpPhysicalObject.Keys.Condition] =
                (long)(targetPhysicalObject.Data.GetLong(CpPhysicalObject.Keys.Condition) - damageDealt);

            //let's do armor soon!
            ////maybe we'll optimize this later
            //var targetEquipment = ContainerSystem
            //    .GetEntityIdsFromFirstContainerByDesc(rgs, targetId, CpContainer.Vals.ContainerDescription.Equipped);

            ////for this simple game, there's only one piece of armor, but we'll use SelectMany anyhow.
            //var armor = targetEquipment.SelectMany(eid => rgs.GetComponentsOfType(eid, "MWCTODO:ARMOR")).FirstOrDefault();

            results.Add($"Adjusted damage: {damageDealt}.");

            var attackerNames = EntityNameSystem.GetEntityNames(rgs, attackerId);
            var targetNames = EntityNameSystem.GetEntityNames(rgs, targetId);

            var attackerProperName = attackerNames.GetString(CpEntityName.Keys.ProperName);
            var targetProperName = targetNames.GetString(CpEntityName.Keys.ProperName);
            var targetConditionString = PhysicalObjectSystem.GetLivingThingConditionDesc(targetPhysicalObject.Data.GetLong(CpPhysicalObject.Keys.Condition));

            var attackResultsMessage = damageDealt > 0 
                ? $"{attackerProperName} swings and hits {targetProperName}. {targetProperName} is {targetConditionString}." 
                : $"{attackerProperName} blunders about ineffectually, and {targetProperName} takes heart.";

            results.Add(attackResultsMessage);

            if (targetConditionString == "dead") combatFinished = true;

            return results;
        }


    }
}
