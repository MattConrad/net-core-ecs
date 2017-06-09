using System;
using System.Collections.Generic;
using System.Linq;
using SampleGame;

namespace NetCoreEcsConsole
{
    class Program
    {
        private static Dictionary<long, List<ConsoleKeyAndChoice>> _agentToActionMappings = new Dictionary<long, List<ConsoleKeyAndChoice>>();
        //melee and ranged targets might be different, but we're going to unify them into a single dictionary for cki purposes.
        //  there is no good reason that the same target as ranged would want a different keystroke or description than itself as a melee target.
        private static Dictionary<long, Dictionary<long, ConsoleKeyToTarget>> _agentToTargetMappings = new Dictionary<long, Dictionary<long, ConsoleKeyToTarget>>();

        static void Main(string[] args)
        {
            var game = new Game(SendPlayerInput);

            //this might not even happen until the game has started and presented the player with an initial scenario.

            foreach(var outputs in game.RunGame())
            {
                foreach (var output in outputs)
                {
                    Console.WriteLine(output.Data);
                }
            }

            Console.WriteLine();
            Console.WriteLine("GAME OVER");
            Console.ReadLine();
        }

        public static string SendPlayerInput(Dictionary<string, string> heroActionDict)
        {
            var actionInputSet = GetKeysToActions(heroActionDict);
            return GetHeroAction(actionInputSet);
        }

        public static ActionChosen SendPlayerInput2(CombatChoicesAndTargets choicesAndTargets)
        {
            var keysToActions = GetKeysToActions2(choicesAndTargets);
            var keysToMeleeTargets = GetKeysToTargets(choicesAndTargets, SampleGame.Vals.CombatCategory.MeleeTarget);
            var keysToRangedTargets = GetKeysToTargets(choicesAndTargets, SampleGame.Vals.CombatCategory.RangedTarget);

            throw new NotImplementedException();
            //return GetHeroAction2(actionInputSet);
        }

        private static List<ConsoleKeyToTarget> GetKeysToTargets(CombatChoicesAndTargets choicesAndTargets, string meleeOrRanged)
        {
            long agentId = choicesAndTargets.Choices.Select(c => c.AgentId).Distinct().Single();
            var establishedTargets = _agentToTargetMappings[agentId];

            var ckiToTarget = new List<ConsoleKeyToTarget>();

            var targetList = meleeOrRanged == SampleGame.Vals.CombatCategory.MeleeTarget ? choicesAndTargets.MeleeTargets : choicesAndTargets.RangedTargets;

            foreach(var kvp in targetList)
            {
                if (establishedTargets.ContainsKey(kvp.Key))
                {
                    ckiToTarget.Add(new ConsoleKeyToTarget { Cki = establishedTargets[kvp.Key].Cki, TargetId = kvp.Key, TargetDescription = kvp.Value });
                }
                else
                {
                    var sameLevelChars = new HashSet<char>(establishedTargets.Select(et => et.Value.Cki.KeyChar));

                    var nextOpenConsoleKey = GetNextOpenConsoleKey(new HashSet<char>(), sameLevelChars);

                    ckiToTarget.Add(new ConsoleKeyToTarget { Cki = nextOpenConsoleKey, TargetId = kvp.Key, TargetDescription = kvp.Value });
                }
            }

            return ckiToTarget;
        }

        static Dictionary<char, TextActionPair> GetKeysToActions(Dictionary<string, string> heroActionDict)
        {
            Dictionary<char, TextActionPair> keyToTAP = new Dictionary<char, TextActionPair>();

            int i = 49;
            foreach (string text in heroActionDict.Keys)
            {
                string action = heroActionDict[text];
                char letter = Convert.ToChar(i);
                i++;

                keyToTAP.Add(letter, new TextActionPair { Text = text, Action = action });

                if (i == 58) i = 65;
                if (i > 90) throw new InvalidOperationException("You were supposed to rewrite this long ago.");
            }

            return keyToTAP;
        }

        //oh god this is terrible, but i just want to move on from it for now.
        //  we need keys to be unique FOR THE SAME GROUP of actions, but they don't need to be unique globally.
        static List<ConsoleKeyAndChoice> GetKeysToActions2(CombatChoicesAndTargets choicesAndTargets)
        {
            var keysAndChoices = new List<ConsoleKeyAndChoice>();

            //for now (probably always) we support only a single agent at a time.
            long agentId = choicesAndTargets.Choices.Select(c => c.AgentId).Distinct().Single();
            var establishedKeysAndChoices = _agentToActionMappings[agentId];

            //ugh, should have a better way of dealing with presets, but this is getting thrown away anyway.
            var presetChars = new HashSet<char>(new char[] { 'm', 'f', 'n', 'r', 'c', 's' });

            foreach (var choice in choicesAndTargets.Choices)
            {
                ConsoleKeyAndChoice presetCkiAndChoice = GetPresetCkiAndChoice(choice);
                if (presetCkiAndChoice != null)
                {
                    keysAndChoices.Add(presetCkiAndChoice);
                    continue;
                }

                ConsoleKeyAndChoice matchingCkiAndChoice = GetEstablishedKeyAndChoice(establishedKeysAndChoices, choice);
                if (matchingCkiAndChoice != null)
                {
                    keysAndChoices.Add(matchingCkiAndChoice);
                    continue;
                }

                var sameLevelChars = new HashSet<char>(establishedKeysAndChoices.Where(c => c.Choice.Category == choice.Category).Select(c => c.Cki.KeyChar));

                var openConsoleKey = GetNextOpenConsoleKey(presetChars, sameLevelChars);

                var newCkiAndChoice = new ConsoleKeyAndChoice { Cki = openConsoleKey, Choice = choice };

                establishedKeysAndChoices.Add(newCkiAndChoice);
            }

            return keysAndChoices;
        }

        private static ConsoleKeyInfo GetNextOpenConsoleKey(HashSet<char> presetChars, HashSet<char> sameLevelChars)
        {
            for (int i = 97; i <= 122; i++)
            {
                char letter = Convert.ToChar(i);

                if (presetChars.Contains(letter)) continue;

                ConsoleKey consoleKey;
                if (Enum.TryParse(letter.ToString().ToUpper(), true, out consoleKey))
                {
                    return new ConsoleKeyInfo(letter, consoleKey, false, false, false);
                }
            }

            throw new InvalidOperationException("Iteration ran to completion unexpectedly, ran out of choices or programming error.");
        }

        //we want the choices to stay as consistent as possible from round to round. if the same action was visible previously, keep it.
        private static ConsoleKeyAndChoice GetEstablishedKeyAndChoice(List<ConsoleKeyAndChoice> establishedChoices, AgentActionChoice choice)
        {
            var establishedChoice = establishedChoices.SingleOrDefault(c => c.Choice.Category == choice.Category 
                && c.Choice.Action == choice.Action && c.Choice.WeaponEntityId == choice.WeaponEntityId && c.Choice.WeaponHandIndex == c.Choice.WeaponHandIndex);

            if (establishedChoice == null) return null;

            //update the description, it could have changed.
            establishedChoice.Choice.Description = choice.Description;

            return establishedChoice;
        }

        private static ConsoleKeyAndChoice GetPresetCkiAndChoice(AgentActionChoice choice)
        {
            if (choice.Category != SampleGame.Vals.CombatCategory.TopLevelAction) return null;

            ConsoleKeyInfo cki;

            //some toplevel actions have an .Action
            if (choice.Action == SampleGame.Vals.CombatAction.AttackWeaponMelee) cki = new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false);
            if (choice.Action == SampleGame.Vals.CombatAction.AttackWeaponRanged) cki = new ConsoleKeyInfo('f', ConsoleKey.F, false, false, false);
            if (choice.Action == SampleGame.Vals.CombatAction.DoNothing) cki = new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false);
            if (choice.Action == SampleGame.Vals.CombatAction.RunAway) cki = new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false);

            //other top levels are inferred by .NextCategory.
            if (choice.NextCategory == SampleGame.Vals.CombatCategory.AllMelee) cki = new ConsoleKeyInfo('m', ConsoleKey.M, false, true, false);
            if (choice.NextCategory == SampleGame.Vals.CombatCategory.AllRanged) cki = new ConsoleKeyInfo('f', ConsoleKey.F, false, true, false);
            if (choice.NextCategory == SampleGame.Vals.CombatCategory.AllSpells) cki = new ConsoleKeyInfo('c', ConsoleKey.C, false, true, false);
            if (choice.NextCategory == SampleGame.Vals.CombatCategory.AllStances) cki = new ConsoleKeyInfo('s', ConsoleKey.S, false, true, false);

            return new ConsoleKeyAndChoice { Cki = cki, Choice = choice };
        }

        static string GetHeroAction(Dictionary<char, TextActionPair> keyToTextActionPair)
        {
            Console.WriteLine("Choose an action:");
            foreach(char key in keyToTextActionPair.Keys)
            {
                Console.WriteLine($"{key}: {keyToTextActionPair[key].Text}");
            }

            ConsoleKeyInfo cki;
            while (!keyToTextActionPair.ContainsKey(cki.KeyChar))
            {
                cki = Console.ReadKey(true);
            }

            return keyToTextActionPair[cki.KeyChar].Action;
        }
    }

    public class TextActionPair
    {
        public string Text { get; set; }
        public string Action { get; set; }
    }

    public class ConsoleKeyAndChoice
    {
        public ConsoleKeyInfo Cki { get; set; }
        public AgentActionChoice Choice { get; set; }
    }

    public class ConsoleKeyToTarget
    {
        public ConsoleKeyInfo Cki { get; set; }
        public long TargetId { get; set; }
        public string TargetDescription { get; set; }
    }


}