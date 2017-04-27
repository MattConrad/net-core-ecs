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

            results.AddRange(ResolveAction(rgs, heroId, villianId, Rando.GetRange5, out combatFinished));

            if (!combatFinished) results.AddRange(ResolveAction(rgs, villianId, heroId, Rando.GetRange5, out combatFinished));

            return results;
        }

        // do we want to pass in the random function for better testing? not sure, but let's try it.
        private static List<string> ResolveAction(EcsRegistrar rgs, long attackerId, long targetId, Func<int> random0to5, out bool combatFinished)
        {
            combatFinished = false;
            var results = new List<string>();

            int attackRoll = random0to5();
            results.Add($"Attacker rolled a {attackRoll}.");

            var targetPhysicalObject = rgs.GetPartsOfType<Parts.PhysicalObject>(targetId).Single();

            //ew, magic strings, fix this.
            var targetEquipment = Container.GetEntityIdsFromFirstContainerByDesc(rgs, targetId, "pack");
            //for this simple game, there's only one piece of armor, but we'll use SelectMany over the entire inventory anyhow.
            // later, this needs to be equipped only. not sure how we're doing equipped right now.
            var targetArmor = targetEquipment.SelectMany(eid => rgs.GetPartsOfType<Parts.DamagePreventer>(eid)).FirstOrDefault();

            //later, we will have natural weapons and whatever you're wielding, and you probably only get one attack at a time.
            //  this relates to the clock/timer, however that ends up working.
            //there you go with that magic string talk again.
            var attackerEquipment = Container.GetEntityIdsFromFirstContainerByDesc(rgs, attackerId, "pack");
            //and here, this needs to be wielded weapons only. perhaps equipped will do intermediately.
            var attackerWeapon = targetEquipment.SelectMany(eid => rgs.GetPartsOfType<Parts.SingleTargetDamager>(eid)).FirstOrDefault();

            var damageAttempted = (attackerWeapon?.DamageAmount ?? 0) * GetDamageMultiplierFromRange5(Math.Max(0, attackRoll));
            var damageDealt = damageAttempted - Math.Min(0, (targetArmor?.DamageThreshold ?? 0));

            //we will need to rethink dodging. for now, there is no defense roll.
            //int defenseRoll = random0to5();
            //results.Add($"Defender rolled a {defenseRoll}.");

            ////eventually we'll want to watch for crits and fumbles
            ////there will be a variety of modifiers, temp and perm, that can apply here.
            //var modifiedDefenseRoll = Math.Max(defenseRoll, 0);
            //var netAttack = Math.Max(attackRoll - modifiedDefenseRoll, 0);

            ////for now, we'll just multiply base roll by 1000, so doing 0-10 points damage in a turn, modified.
            //var damageDealt = ((netAttack * 1000) - targetPhysicalObject.DefaultDamageThreshold) 
            //    * targetPhysicalObject.DefaultDamageMultiplier;

            targetPhysicalObject.Condition = targetPhysicalObject.Condition - (int)damageDealt;

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

        //this is just some made up stuff, i have no idea how to design a combat system.
        private static decimal GetDamageMultiplierFromRange5(int roll)
        {
            return (roll == 5)
                ? 4m
                : (roll == 4)
                    ? 2m
                    : (roll > 1)
                        ? 1m
                        : (roll == 1)
                            ? 0.5m
                            : 0m;
        }


    }
}
