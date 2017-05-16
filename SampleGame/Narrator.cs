﻿using System;
using System.Collections.Generic;
using System.Text;
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
                var result = new Output { Category = OutputCategory.Text };

                var sb = new StringBuilder();

                var attNames = rgs.GetPartSingle<Parts.EntityName>(msg.ActorId);
                var tgtNames = rgs.GetPartSingle<Parts.EntityName>(msg.TargetId);

                sb.Append($"{attNames.ProperName} makes a {msg.ActorAdjustedRoll} attack");
                sb.Append(msg.AttemptedDamage > 0 ? " and connects.  " : $", but {tgtNames.ProperName} ducks out of the way.");
                var tgtConditionString = GetLivingThingConditionDesc(msg.TargetCondition);

                if (msg.AttemptedDamage > 0 && msg.NetDamage == 0)
                {
                    sb.Append($"Unfortunately for {attNames.ProperName}, the attack isn't solid, and {tgtNames.ProperName}'s armor protects {tgtNames.Pronoun} entirely.  ");
                }
                else if (msg.NetDamage > 7000)
                {
                    result.Data = sb.ToString();
                    results.Add(result);
                    results.Add(new Output { Category = OutputCategory.Text, Data = "" });

                    result = new Output { Category = OutputCategory.Text, Data = $"The attack is nearly perfect, and it is devastating. {tgtNames.ProperName} is {tgtConditionString}.  " };
                }
                else if (msg.NetDamage > 4000)
                {
                    sb.Append($"It's a fine strike, penetrating the armor and inflicting a nasty wound. {tgtNames.ProperName} is {tgtConditionString}.  ");
                }
                else if (msg.NetDamage > 1900)
                {
                    sb.Append($"A hit . . . hard enough to notice");

                    if (msg.TargetCondition > 4000)
                    {
                        sb.Append($", but not enough to seriously injure. {tgtNames.ProperName} is {tgtConditionString}.");
                    }
                    else if (msg.TargetCondition > 2000)
                    {
                        sb.Append($". {tgtNames.ProperName} is {tgtConditionString}.");
                    }
                    else if (msg.TargetCondition > 0)
                    {
                        sb.Append($", and at this point, that means trouble. {tgtNames.ProperName} is {tgtConditionString}.");
                    }
                    else
                    {
                        sb.Append($" . . . momentarily. {tgtNames.ProperName} would have laughed at a strike like this earlier, but this time {tgtNames.Pronoun} watches in horror as the point slides home. {tgtNames.ProperName} dies.");
                    }
                }

                result.Data = sb.ToString();
            }

            return results;
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