using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Combat
    {
        internal static List<string> HerosAction(EcsRegistrar rgs, long heroId, long villianId, out bool combatFinished)
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

            int attackRoll = Rando.GetRange5();
            results.Add($"Attacker rolled a {attackRoll}.");

            int defenseRoll = Rando.GetRange5();
            results.Add($"Defender rolled a {defenseRoll}.");

            //eventually we'll want to watch for crits and fumbles
            //there will be a variety of modifiers, temp and perm, that can apply here.
            var modifiedDefenseRoll = Math.Max(defenseRoll, 0);
            var netAttack = Math.Max(attackRoll - modifiedDefenseRoll, 0);

            var targetPhysicalObject = rgs.GetPartsOfType<Parts.PhysicalObject>(targetId).Single();

            //for now, we'll just multiply base roll by 1000, so doing 0-10 points damage in a turn, modified.
            var damageDealt = ((netAttack * 1000) - targetPhysicalObject.DefaultDamageThreshold) * targetPhysicalObject.DefaultDamageMultiplier;

            targetPhysicalObject.Condition = targetPhysicalObject.Condition - (int)damageDealt;

            //let's do armor soon!
            ////maybe we'll optimize this later
            //var targetEquipment = ContainerSystem
            //    .GetEntityIdsFromFirstContainerByDesc(rgs, targetId, CpContainer.Vals.ContainerDescription.Equipped);

            ////for this simple game, there's only one piece of armor, but we'll use SelectMany anyhow.
            //var armor = targetEquipment.SelectMany(eid => rgs.GetComponentsOfType(eid, "MWCTODO:ARMOR")).FirstOrDefault();

            results.Add($"Adjusted damage: {damageDealt}.");

            var attackerNames = rgs.GetPartsOfType<Parts.EntityName>(attackerId).Single();
            var targetNames = rgs.GetPartsOfType<Parts.EntityName>(targetId).Single();

            var attackerProperName = attackerNames.ProperName;
            var targetProperName = targetNames.ProperName;
            var targetConditionString = Sys.PhysicalObject.GetLivingThingConditionDesc(targetPhysicalObject.Condition);

            var attackResultsMessage = damageDealt > 0
                ? $"{attackerProperName} swings and hits {targetProperName}. {targetProperName} is {targetConditionString}."
                : $"{attackerProperName} blunders about ineffectually, and {targetProperName} takes heart.";

            results.Add(attackResultsMessage);

            if (targetConditionString == "dead") combatFinished = true;

            return results;
        }
    }
}
