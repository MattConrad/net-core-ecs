using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Combat
    {
        internal static readonly string[] Stances = new string[] { Vals.CombatAction.StanceAggressive, Vals.CombatAction.StanceDefensive, Vals.CombatAction.StanceStandGround };
        internal static readonly string[] Incapacitations = new string[] { Vals.CombatStatusTag.Dead };

        internal static List<Messages.Combat> ProcessAgentAction(EcsRegistrar rgs, long agentId, long? targetId, string action)
        {
            if (Stances.Contains(action))
            {
                return ApplyStance(rgs, agentId, action);
            }
            else if (action == Vals.CombatAction.SwitchToAI)
            {
                return SwitchToAI(rgs, agentId);
            }
            else if (action == Vals.CombatAction.AttackMelee)
            {
                return ResolveSingleTargetMelee(rgs, agentId, targetId.Value);
            }
            else if (action == Vals.CombatAction.DoNothing)
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
            if (stance == Vals.CombatAction.StanceAggressive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier {
                    Tag = Vals.CombatAction.StanceAggressive,
                    SkillDeltas = new Dictionary<string, int> {
                        [Vals.EntitySkillPhysical.Melee] = 1,
                        [Vals.EntitySkillPhysical.Dodge] = -1
                    }
                });
            }
            else if (stance == Vals.CombatAction.StanceDefensive)
            {
                rgs.AddPart(agentId, new Parts.SkillsModifier
                {
                    Tag = Vals.CombatAction.StanceDefensive,
                    SkillDeltas = new Dictionary<string, int>
                    {
                        [Vals.EntitySkillPhysical.Melee] = -1,
                        [Vals.EntitySkillPhysical.Dodge] = 1
                    }
                });

            }
            else if (stance == Vals.CombatAction.StanceStandGround)
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

        //MWCTODO: we rewrote a bunch of stuff, but does any of it do what we meant?
        // do we want to pass in the random function for better testing? not sure, but let's try it.
        public static List<Messages.Combat> ResolveSingleTargetMelee(EcsRegistrar rgs, long attackerId, long targetId, Func<int> random0to5)
        {
            //this is only temporary, later we'll have a clock/scheduler.
            var msg = new Messages.Combat
            {
                Tick = rgs.NewId(),
                ActorId = attackerId,
                TargetId = targetId,
                ActorAction = Vals.CombatAction.AttackMelee,
                TargetAction = Vals.CombatAction.Dodge
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
            var targetEquipmentIds = rgs.GetPartSingle<Parts.Anatomy>(targetId).SlotsEquipped.Where(s => s.Value != 0).Select(s => s.Value).ToList();

            var targetDamagePreventers = targetEquipmentIds.SelectMany(eid => rgs.GetParts<Parts.DamagePreventer>(eid)).ToList();

            //later, we will have natural weapons and whatever you're wielding, and you probably only get one attack at a time. also, watch for shield in first slot (or maybe player has to).
            //  this relates to the clock/timer, however that ends up working.
            var attackerWieldingSlots = rgs.GetPartSingle<Parts.Anatomy>(attackerId).SlotsEquipped
                .Where(s => s.Key == Vals.BodyParts.WieldObjectAppendage && s.Value != 0)
                .ToList();

            long attackerWeaponId = attackerWieldingSlots.Any() ? attackerWieldingSlots.First().Value : 0;

            //MWCTODO: this would be a place to handle natural weaponry.
            var attackerDamagers = attackerWeaponId == 0L ? new List<Parts.Damager>() : rgs.GetParts<Parts.Damager>(attackerWeaponId).ToList();

            //MWCTODO: this doesn't address damage types at all. that's going to be . . . interesting when you get there.
            var damageAttempted = attackerDamagers.Sum(d => d.DamageAmount) * finalAttackMultiplier;
            var damagePrevented = targetPhysicalObject.DefaultDamageThreshold + targetDamagePreventers.Sum(p => p.DamageThreshold);
            msg.AttemptedDamage = damageAttempted;

            //so we apply crit/attack multipliers first, then we subtract damage prevention, then we apply default damage multiplier and armor multipliers. whew!
            var damageDealt = Math.Max(0, 
                (damageAttempted - damagePrevented) 
                * targetPhysicalObject.DefaultDamageMultiplier 
                * (targetDamagePreventers.Select(p => p.DamageMultiplier).Aggregate(1m, (p1, p2) =>  p1 * p2)));
            msg.NetDamage = damageDealt;

            targetPhysicalObject.Condition = targetPhysicalObject.Condition - (int)damageDealt;
            msg.TargetCondition = targetPhysicalObject.Condition;

            if (targetPhysicalObject.Condition <= 0)
            {
                var targetAgent = rgs.GetPartSingle<Parts.Agent>(targetId);

                if (!targetAgent.CombatStatusTags.Contains(Vals.CombatStatusTag.Dead))
                {
                    targetAgent.CombatStatusTags.Add(Vals.CombatStatusTag.Dead);
                    msg.NewTargetCombatTags.Add(Vals.CombatStatusTag.Dead);
                }
            }

            return new List<Messages.Combat> { msg };
        }

        private static List<Messages.Combat> SwitchToAI(EcsRegistrar rgs, long agentId)
        {
            var ai = rgs.GetPartSingle<Parts.Agent>(agentId);
            ai.ActiveCombatAI = Vals.AI.MeleeOnly;
            return new List<Messages.Combat> {
                    new Messages.Combat { Tick = rgs.NewId(), ActorId = agentId, ActorAction = "Switched to AI." }
            };
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
