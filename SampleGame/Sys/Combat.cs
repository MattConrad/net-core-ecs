using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Combat
    {
        internal static readonly string[] Stances = new string[] { Vals.CombatActions.StanceAggressive, Vals.CombatActions.StanceDefensive, Vals.CombatActions.StanceStandGround };

        internal static List<Messages.Combat> ProcessAgentAction(EcsRegistrar rgs, long agentId, long? targetId, string action)
        {
            if (Stances.Contains(action))
            {
                return ApplyStance(rgs, agentId, action);
            }
            else if (action == Vals.CombatActions.SwitchToAI)
            {
                var ai = rgs.GetPartSingle<Parts.Agent>(agentId);
                ai.ActiveCombatAI = Vals.AI.MeleeOnly;
                return new List<Messages.Combat> {
                    new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = "Switched to AI." }
                };
            }
            else if (action == Vals.CombatActions.AttackMelee)
            {
                return ResolveSingleTargetMelee(rgs, agentId, targetId.Value);
            }
            else if (action == Vals.CombatActions.DoNothing)
            {
                var agentNames = rgs.GetPartSingle<Parts.EntityName>(agentId);
                return new List<Messages.Combat> {
                    new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = $"{agentNames.ProperName} stands still and looks around in confusion. Probably some programmer failed to give him a good action in this scenario." }
                };
            }
            else
            {
                throw new ArgumentException($"Action '{action}' not supported.");
            }
        }

        internal static List<Messages.Combat> ApplyStance(EcsRegistrar rgs, long agentId, string stance)
        {
            var result = new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId };
            var stances = rgs.GetParts<Parts.SkillsModifier>(agentId).Where(p => Stances.Contains(p.Tag));
            rgs.RemoveParts(agentId, stances);

            result.ActorAction = stance;
            if (stance == Vals.CombatActions.StanceAggressive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier {
                    Tag = Vals.CombatActions.StanceAggressive,
                    SkillDeltas = new Dictionary<string, int> {
                        [Vals.EntitySkillPhysical.Melee] = 1,
                        [Vals.EntitySkillPhysical.Dodge] = -1
                    }
                });
            }
            else if (stance == Vals.CombatActions.StanceDefensive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier
                {
                    Tag = Vals.CombatActions.StanceDefensive,
                    SkillDeltas = new Dictionary<string, int>
                    {
                        [Vals.EntitySkillPhysical.Melee] = -1,
                        [Vals.EntitySkillPhysical.Dodge] = 1
                    }
                });

            }
            else if (stance == Vals.CombatActions.StanceStandGround)
            {
                //default stance, so the removal of previous ones means we're done.
            }
            else
            {
                throw new ArgumentException($"Invalid stance {stance}.");
            }

            return new List<Messages.Combat> { result };
        }

        public static List<Messages.Combat> ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId)
        {
            return ResolveSingleTargetMelee(rgs, attackerId, targetId, Rando.GetRange5);
        }

        // do we want to pass in the random function for better testing? not sure, but let's try it.
        public static List<Messages.Combat> ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId, Func<int> random0to5)
        {
            //this is only temporary, later we'll have a clock/scheduler.
            var msg = new Messages.Combat
            {
                Tick = rgs.NewId(),
                ActorId = attackerId,
                TargetId = targetId,
                ActorAction = Vals.CombatActions.AttackMelee,
                TargetAction = Vals.CombatActions.Dodge
            };

            var attackerAdjustedSkills = Skills.GetAdjustedSkills(rgs, attackerId);
            var targetAdjustedSkills = Skills.GetAdjustedSkills(rgs, targetId);

            int attackRoll = random0to5();
            decimal attackCritMultiplier = GetDamageMultiplierFromRange5(attackRoll);
            int attackerMeleeSkill = attackerAdjustedSkills[Vals.EntitySkillPhysical.Melee];
            int attackerAdjustedRoll = attackRoll + attackerMeleeSkill;
            msg.ActorAdjustedSkill = attackerMeleeSkill;
            msg.ActorAdjustedRoll = attackerAdjustedRoll;

            int targetDodgeRoll = random0to5();
            int targetDodgeSkill = targetAdjustedSkills[Vals.EntitySkillPhysical.Dodge];
            //dodge is a difficult skill and always reduced by 1.
            int targetAdjustedDodgeRoll = Math.Max(0, targetDodgeRoll + targetDodgeSkill - 1);
            msg.TargetAdjustedSkill = targetDodgeSkill;
            msg.TargetAdjustedRoll = targetAdjustedDodgeRoll;

            int netAttack = Math.Max(0, attackerAdjustedRoll - targetAdjustedDodgeRoll);
            msg.NetActorRoll = netAttack;

            //a good attack gets whatever the attack crit damage multiplier is, a barely-attack gets a .5, and less gets a 0.
            decimal finalAttackMultiplier = (netAttack > 1) ? attackCritMultiplier 
                : (netAttack == 1) ? 0.5m
                : 0m;

            var targetPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(targetId);
            var targetEquipment = Container.GetEntityIdsFromFirstTagged(rgs, targetId, Vals.ContainerTag.Equipped);

            //for this simple game, there's only one piece of armor, but we'll use SelectMany over the entire equipment anyhow.
            // later this is certainly true, maybe over more than equipment. lots of things can be damage preventers. eeeps.
            // later, this needs to be equipped only. not sure how we're doing equipped right now.
            var targetArmor = targetEquipment.SelectMany(eid => rgs.GetParts<Parts.DamagePreventer>(eid)).FirstOrDefault();

            //later, we will have natural weapons and whatever you're wielding, and you probably only get one attack at a time.
            //  this relates to the clock/timer, however that ends up working.
            var attackerEquipment = Container.GetEntityIdsFromFirstTagged(rgs, attackerId, Vals.ContainerTag.Equipped);
            //and here, this needs to be wielded weapons only. perhaps equipped will do intermediately.
            var attackerWeapon = targetEquipment.SelectMany(eid => rgs.GetParts<Parts.Damager>(eid)).FirstOrDefault();

            var damageAttempted = (attackerWeapon?.DamageAmount ?? 0) * finalAttackMultiplier;
            var damagePrevented = targetPhysicalObject.DefaultDamageThreshold + targetArmor?.DamageThreshold ?? 0;
            msg.AttemptedDamage = damageAttempted;

            //so we apply crit/attack multipliers first, then we subtract damage prevention, then we apply default damage multiplier and armor multiplier. whew!
            var damageDealt = Math.Max(0, 
                (damageAttempted - damagePrevented) * targetPhysicalObject.DefaultDamageMultiplier * targetArmor.DamageMultiplier);
            msg.NetDamage = damageDealt;

            targetPhysicalObject.Condition = targetPhysicalObject.Condition - (int)damageDealt;
            msg.TargetCondition = targetPhysicalObject.Condition;

            return new List<Messages.Combat> { msg };
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
