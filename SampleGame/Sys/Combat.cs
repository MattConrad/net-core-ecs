using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Combat
    {
        internal static class Actions
        {
            internal const string AttackMelee = "attack-melee";
            internal const string AttackMeleeContinously = "attack-continuously";
            internal const string StanceDefensive = "stance-defensive";
            internal const string StanceStandGround = "stance-stand-ground";
            internal const string StanceAggressive = "stance-aggressive";
        }

        internal static readonly string[] Stances = new string[] { Actions.StanceAggressive, Actions.StanceDefensive, Actions.StanceStandGround };

        internal static List<string> HerosAction(EcsRegistrar rgs, long heroId, long targetId, string heroAction, out bool combatFinished)
        {
            List<string> results = new List<string>();

            do
            {
                if (Stances.Contains(heroAction))
                {
                    results.AddRange(ApplyStance(rgs, heroId, heroAction));

                    //taking a stance is an action, so the bad guys get their turn next.
                    results.AddRange(ResolveSingleTargetMelee(rgs, targetId, heroId, Rando.GetRange5, out combatFinished));
                }
                else if (heroAction == Actions.AttackMelee || heroAction == Actions.AttackMeleeContinously)
                {
                    results.AddRange(ResolveSingleTargetMelee(rgs, heroId, targetId, Rando.GetRange5, out combatFinished));

                    if (!combatFinished) results.AddRange(ResolveSingleTargetMelee(rgs, targetId, heroId, Rando.GetRange5, out combatFinished));
                }
                else
                {
                    combatFinished = false;
                    results.Add($"Sorry, heroes can't {heroAction} yet.");
                }
            } while (!combatFinished && heroAction == Actions.AttackMeleeContinously);

            return results;
        }

        internal static List<string> ApplyStance(EcsRegistrar rgs, long agentId, string stance)
        {
            List<string> results = new List<string>();

            var stances = rgs.GetPartsOfType<Parts.SkillsModifier>(agentId).Where(p => Stances.Contains(p.Tag));
            rgs.RemoveParts(agentId, stances);

            if (stance == Actions.StanceAggressive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier {
                    Tag = Actions.StanceAggressive,
                    SkillDeltas = new Dictionary<string, int> {
                        [Parts.Skillset.Vals.Physical.Melee] = 1,
                        [Parts.Skillset.Vals.Physical.Dodge] = -1
                    }
                });
                results.Add($"[Agent] assumes an aggressive stance.");
            }
            else if (stance == Actions.StanceDefensive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier
                {
                    Tag = Actions.StanceDefensive,
                    SkillDeltas = new Dictionary<string, int>
                    {
                        [Parts.Skillset.Vals.Physical.Melee] = -1,
                        [Parts.Skillset.Vals.Physical.Dodge] = 1
                    }
                });
                results.Add($"[Agent] assumes a defensive stance.");
            }
            else if (stance == Actions.StanceStandGround)
            {
                results.Add($"[Agent] stands [its] ground.");
            }
            else
            {
                results.Add($"Sorry, agents can't take stance {stance} yet.");
            }

            return results;
        }

        // do we want to pass in the random function for better testing? not sure, but let's try it.
        private static List<string> ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId, Func<int> random0to5, out bool combatFinished)
        {
            combatFinished = false;
            var results = new List<string>();

            var attackerParts = rgs.GetAllParts(attackerId);
            var targetParts = rgs.GetAllParts(targetId);

            var attackerNames = attackerParts.OfType<Parts.EntityName>().Single();
            var targetNames = targetParts.OfType<Parts.EntityName>().Single();

            string attackerProperName = attackerNames.ProperName;
            string targetProperName = targetNames.ProperName;

            var attackerSkills = attackerParts.OfType<Parts.Skillset>().Single();
            var targetSkills = targetParts.OfType<Parts.Skillset>().Single();

            int attackRoll = random0to5();
            decimal attackCritMultiplier = GetDamageMultiplierFromRange5(attackRoll);
            string attackRollAdjective = GetRollAdjectiveFromRange5(attackRoll);
            int attackerMeleeSkill = attackerSkills.Skills[Parts.Skillset.Vals.Physical.Melee];
            int adjustedAttackRoll = attackRoll + attackerMeleeSkill;

            int targetDodgeRoll = random0to5();
            string targetDodgeRollAdjective = GetRollAdjectiveFromRange5(targetDodgeRoll);
            int targetDodgeSkill = targetSkills.Skills[Parts.Skillset.Vals.Physical.Dodge];
            //dodge is a difficult skill and always reduced by 1.
            int adjustedDodgeRoll = Math.Max(0, targetDodgeRoll + targetDodgeSkill - 1);

            int netAttack = Math.Max(0, adjustedAttackRoll - adjustedDodgeRoll);

            results.Add($"{attackerProperName} makes {attackRollAdjective} attack, and {targetProperName} responds with a {targetDodgeRollAdjective} dodge.");

            //MWCTODO: now stances need to actually do something.

            //a good attack gets whatever the attack crit damage multiplier is, a barely-attack gets a .5, and less gets a 0.
            decimal finalAttackMultiplier = (netAttack > 1) ? attackCritMultiplier 
                : (netAttack == 1) ? 0.5m
                : 0m;

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
            var attackerWeapon = targetEquipment.SelectMany(eid => rgs.GetPartsOfType<Parts.Damager>(eid)).FirstOrDefault();

            var damageAttempted = (attackerWeapon?.DamageAmount ?? 0) * finalAttackMultiplier;
            var damagePrevented = targetPhysicalObject.DefaultDamageThreshold + targetArmor?.DamageThreshold ?? 0;
            //so we apply crit/attack multipliers first, then we subtract damage prevention, then we apply default damage multiplier and armor multiplier. whew!
            var damageDealt = Math.Max(0, 
                (damageAttempted - damagePrevented) * targetPhysicalObject.DefaultDamageMultiplier * targetArmor.DamageMultiplier);

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
