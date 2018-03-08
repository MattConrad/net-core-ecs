using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    internal class Narrator
    {
        internal static List<Output> OutputForCombatMessages(EcsRegistrar rgs, List<Messages.Combat> messages)
        {
            var results = new List<Output>();

            foreach(var msg in messages)
            {
                //MWCTODO: add a debug switch for this info
                results.Add(new Output { Category = OutputCategory.Text, Data = $"(DEBUG: attempted damage {msg.AttemptedDamage} net damage {msg.NetDamage} critical {msg.ActorCritical} target condition {msg.TargetCondition})" });

                var result = new Output { Category = OutputCategory.Text };

                var sb = new StringBuilder();

                var actorNames = rgs.GetPartSingle<Parts.EntityName>(msg.ActorId);

                if (Vals.CombatAction.AllStances.Contains(msg.ActorAction))
                {
                    result.Data = $"{actorNames.ProperName} assumes a {msg.ActorAction} stance.";
                    results.Add(result);
                }
                else if (msg.ActorAction == Vals.CombatAction.DoNothing)
                {
                    result.Data = $"{actorNames.ProperName} stands still and looks around dumbly. What do?";
                    results.Add(result);
                }
                else if (msg.ActorAction == Vals.CombatAction.RunAway)
                {
                    result.Data = $"{actorNames.ProperName} tries to run away, and {actorNames.Pronoun} feet spin on the floor in a cartoon whirlwind, but nothing actually happens. Uh oh.";
                    results.Add(result);
                }
                else if (msg.TargetId == 0)
                {
                    result.Data = $"MWCTODO: we don't know what {msg.ActorAction} is.";
                    results.Add(result);
                }

                //if the result has data, we're done with this iteration.
                if (!string.IsNullOrEmpty(result.Data)) continue;

                var targetNames = rgs.GetPartSingle<Parts.EntityName>(msg.TargetId);

                sb.Append($"{actorNames.ProperName} {PresentTenseVerb(msg.AttackVerb)} with a {msg.ActorAdjustedRoll} attack.");

                if (msg.ActorAdjustedRoll <= 0)
                {
                    sb.Append($", presenting no real threat to {targetNames.ProperName}, who steps back from it casually.");
                    result.Data = sb.ToString();
                    AppendNoDamageButNearlyDead(sb, targetNames.ProperName, targetNames.Pronoun, msg.TargetCondition);
                    results.Add(result);
                    continue;
                }

                if (msg.AttemptedDamage <= 0)
                {
                    sb.Append($", but {targetNames.ProperName} sees it coming and ducks out of the way, avoiding it entirely.");
                    result.Data = sb.ToString();
                    AppendNoDamageButNearlyDead(sb, targetNames.ProperName, targetNames.Pronoun, msg.TargetCondition);
                    results.Add(result);
                    continue;
                }

                if (msg.NetDamage <= 0)
                {
                    sb.Append($" and connects! Unfortunately for {actorNames.ProperName}, the attack isn't solid, and {targetNames.ProperName}'s armor protects {targetNames.Pronoun} entirely.");
                    result.Data = sb.ToString();
                    AppendNoDamageButNearlyDead(sb, targetNames.ProperName, targetNames.Pronoun, msg.TargetCondition);
                    results.Add(result);
                    continue;
                }

                var targetConditionString = GetLivingThingConditionDesc(msg.TargetCondition);

                //any fatality or any crit ought to get a special fatality/crit message.
                if (msg.NetDamage > 7000)
                {
                    sb.Append($".  The attack is nearly perfect, and it is devastating. {targetNames.ProperName} is {targetConditionString}.");
                }
                else if (msg.NetDamage > 4000)
                {
                    sb.Append($".  It's a fine strike, penetrating the armor and inflicting a nasty wound. {targetNames.ProperName} is {targetConditionString}.  ");
                }
                else if (msg.NetDamage > 1900)
                {
                    sb.Append($".  A hit . . . hard enough to notice. {targetNames.ProperName} is {targetConditionString}.  ");
                }
                else if (msg.NetDamage > 0)
                {
                    if (msg.TargetCondition > 2000)
                    {
                        sb.Append($".  It doesn't amount to much, though. Barely a scratch. It will take a lot of these to wear down {targetNames.ProperName}.");
                    }
                    else if (msg.TargetCondition > 1)
                    {
                        sb.Append($".  Earlier, {targetNames.ProperName} would have laughed at a wound like this. It's not much more than a scratch. {targetNames.ProperName} isn't laughing now. When you're already gravely wounded, any attack that lands might be your last, and {targetNames.ProperName} is already looking death in the eyes.");
                    }
                    else
                    {
                        sb.Append($".  It's not much of an attack, but it didn't need to be. {targetNames.ProperName} watches in horror as the point slides home. {targetNames.Pronoun} lets go of {targetNames.Pronoun} weapon and reaches up for a moment before falling to the ground, dead.");
                    }
                }

                result.Data = sb.ToString();

                results.Add(result);
            }

            return results;
        }

        private static string PresentTenseVerb(string verb)
        {
            if (verb.EndsWith("o") || verb.EndsWith("sh") 
                || verb.EndsWith("ch") || verb.EndsWith("tch") 
                || verb.EndsWith("x") || verb.EndsWith("ss")) return $"{verb}es";

            if (verb.EndsWith("y")) return verb.Substring(0, verb.Length - 1) + "ies";

            return $"{verb}s";
        }

        private static void AppendNoDamageButNearlyDead(StringBuilder sb, string targetProperName, string targetPronoun, long condition)
        {
            if (condition < 2000)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append($"However, {targetProperName} is still in serious trouble. Blood is dripping out of {targetPronoun} armor and {targetPronoun} staggers a little as {targetPronoun} readies a counterattack.");
            }
        }

        //i *think* this belongs here. short term stuff anyway.
        private static string GetLivingThingConditionDesc(long condition)
        {
            if (condition >= 10000) return "untouched";
            if (condition >= 8000) return "bruised and scratched";
            if (condition >= 6000) return "battered and bleeding";
            if (condition >= 4000) return "injured but determined";
            if (condition >= 2000) return "looking fearful";
            if (condition >= 1) return "in desperate straits";

            return "dead";
        }

    }
}
