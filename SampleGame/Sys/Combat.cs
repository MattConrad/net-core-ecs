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

            var attackerParts = rgs.GetAllParts(attackerId);
            var targetParts = rgs.GetAllParts(targetId);

            var attackerNames = attackerParts.OfType<Parts.EntityName>().Single();
            var targetNames = targetParts.OfType<Parts.EntityName>().Single();

            string attackerProperName = attackerNames.ProperName;
            string targetProperName = targetNames.ProperName;

            int attackRoll = random0to5();
            decimal damageMultiplier = GetDamageMultiplierFromRange5(attackRoll);
            string rollAdjective = GetRollAdjectiveFromRange5(attackRoll);
            results.Add($"{attackerProperName} makes {rollAdjective} attack.");

            var attackerSkills = attackerParts.OfType<Parts.Skillset>().Single();
            int meleeSkill = attackerSkills.Skills[Parts.Skillset.Vals.Physical.Melee];
            //later, this will match against dodge.
            damageMultiplier = (attackRoll + meleeSkill > 0) ? damageMultiplier : 0;

            var targetPhysicalObject = targetParts.OfType<Parts.PhysicalObject>().Single();
            var targetEquipment = Container.GetEntityIdsFromFirstTagged(rgs, targetId, Parts.Container.Vals.Tag.Equipped);

            //for this simple game, there's only one piece of armor, but we'll use SelectMany over the entire equipment anyhow.
            // later this is certainly true, maybe over more than equipment. lots of things can be damage preventers. eeeps.
            // later, this needs to be equipped only. not sure how we're doing equipped right now.
            var targetArmor = targetEquipment.SelectMany(eid => rgs.GetPartsOfType<Parts.DamagePreventer>(eid)).FirstOrDefault();

            //later, we will have natural weapons and whatever you're wielding, and you probably only get one attack at a time.
            //  this relates to the clock/timer, however that ends up working.
            var attackerEquipment = Container.GetEntityIdsFromFirstTagged(rgs, attackerId, Parts.Container.Vals.Tag.Equipped);
            //and here, this needs to be wielded weapons only. perhaps equipped will do intermediately.
            var attackerWeapon = targetEquipment.SelectMany(eid => rgs.GetPartsOfType<Parts.SingleTargetDamager>(eid)).FirstOrDefault();

            var damageAttempted = (attackerWeapon?.DamageAmount ?? 0) * damageMultiplier;
            var damagePrevented = targetArmor?.DamageThreshold ?? 0;
            var damageDealt = Math.Max(0, damageAttempted - damagePrevented);

            targetPhysicalObject.Condition = targetPhysicalObject.Condition - (int)damageDealt;

            results.Add($"Adjusted damage: {damageDealt}. New condition: {targetPhysicalObject.Condition}");

            var targetConditionString = PhysicalObject.GetLivingThingConditionDesc(targetPhysicalObject.Condition);

            var attackResultsMessage = damageDealt > 0
                ? $"{attackerProperName} swings and hits {targetProperName}. {targetProperName} is {targetConditionString}."
                : $"{attackerProperName} blunders about ineffectually, and {targetProperName} takes heart.";

            results.Add(attackResultsMessage);

            if (targetConditionString == "dead") combatFinished = true;

            return results;
        }

        private static string GetRollAdjectiveFromRange5(int roll)
        {
            switch(roll)
            {
                case 5: return "an amazing";
                case 4: return "an excellent";
                case -4: return "a poor";
                case -5: return "a miserable";
                default: return "an undistinguished";
            }
        }

        //this is just some made up stuff, i have no idea how to design a combat system.
        private static decimal GetDamageMultiplierFromRange5(int roll)
        {
            //someday crits and fumbles will be more interesting, applying debilities to target or self.
            switch (roll)
            {
                case 5: return 4;
                case 4: return 2;
                case -4: 
                case -5: return 0;
                default: return 1;
            }
        }


    }
}
