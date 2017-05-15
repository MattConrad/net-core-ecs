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
            internal const string SwitchToAI = "switch-to-ai";
            internal const string StanceDefensive = "stance-defensive";
            internal const string StanceStandGround = "stance-stand-ground";
            internal const string StanceAggressive = "stance-aggressive";
        }

        internal static readonly string[] Stances = new string[] { Actions.StanceAggressive, Actions.StanceDefensive, Actions.StanceStandGround };

        internal static Messages.Combat ProcessAgentAction(EcsRegistrar rgs, long agentId, long? targetId, string action)
        {
            if (Stances.Contains(action))
            {
                return ApplyStance(rgs, agentId, action);
            }
            else if (action == Actions.SwitchToAI)
            {
                var ai = rgs.GetPartSingle<Parts.Agent>(agentId);
                ai.ActiveCombatAI = Parts.Agent.Vals.AI.MeleeOnly;
                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = "MWCTODO: Switch to AI" };
            }
            else if (action == Actions.AttackMelee)
            {
                return ResolveSingleTargetMelee(rgs, agentId, targetId.Value);
            }
            else
            {
                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = Messages.TempActionCategories.Stance };
            }
        }

        internal static Messages.Combat ApplyStance(EcsRegistrar rgs, long agentId, string stance)
        {
            var stances = rgs.GetParts<Parts.SkillsModifier>(agentId).Where(p => Stances.Contains(p.Tag));
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

                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = Messages.TempActionCategories.Stance };
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

                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = Messages.TempActionCategories.Stance };
            }
            else if (stance == Actions.StanceStandGround)
            {
                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = Messages.TempActionCategories.Stance };
            }
            else
            {
                return new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = Messages.TempActionCategories.Stance };
            }
        }

        public static Messages.Combat ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId)
        {
            return ResolveSingleTargetMelee(rgs, attackerId, targetId, Rando.GetRange5);
        }

        //MWCTODO: we need to (for now) remove entities that are killed from the battlefield.
        // do we want to pass in the random function for better testing? not sure, but let's try it.
        public static Messages.Combat ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId, Func<int> random0to5)
        {
            //this is only temporary, later we'll have a clock/scheduler.
            var msg = new Messages.Combat
            {
                Tick = rgs.NewId(),
                ActorId = attackerId,
                TargetId = targetId,
                ActorAction = Messages.TempActionCategories.MeleeAttack,
                TargetAction = Messages.TempActionCategories.Dodge
            };

            var attackerAdjustedSkills = Skills.GetAdjustedSkills(rgs, attackerId);
            var targetAdjustedSkills = Skills.GetAdjustedSkills(rgs, targetId);

            int attackRoll = random0to5();
            decimal attackCritMultiplier = GetDamageMultiplierFromRange5(attackRoll);
            int attackerMeleeSkill = attackerAdjustedSkills[Parts.Skillset.Vals.Physical.Melee];
            int attackerAdjustedRoll = attackRoll + attackerMeleeSkill;
            msg.ActorAdjustedSkill = attackerMeleeSkill;
            msg.ActorAdjustedRoll = attackerAdjustedRoll;

            int targetDodgeRoll = random0to5();
            int targetDodgeSkill = targetAdjustedSkills[Parts.Skillset.Vals.Physical.Dodge];
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
            var targetEquipment = Container.GetEntityIdsFromFirstTagged(rgs, targetId, Parts.Container.Vals.Tag.Equipped);

            //for this simple game, there's only one piece of armor, but we'll use SelectMany over the entire equipment anyhow.
            // later this is certainly true, maybe over more than equipment. lots of things can be damage preventers. eeeps.
            // later, this needs to be equipped only. not sure how we're doing equipped right now.
            var targetArmor = targetEquipment.SelectMany(eid => rgs.GetParts<Parts.DamagePreventer>(eid)).FirstOrDefault();

            //later, we will have natural weapons and whatever you're wielding, and you probably only get one attack at a time.
            //  this relates to the clock/timer, however that ends up working.
            var attackerEquipment = Container.GetEntityIdsFromFirstTagged(rgs, attackerId, Parts.Container.Vals.Tag.Equipped);
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


            return msg;
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
