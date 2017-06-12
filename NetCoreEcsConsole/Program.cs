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

        public static ActionChosen SendPlayerInput(CombatChoicesAndTargets choicesAndTargets)
        {
            var keysToActions = GetKeysToActions(choicesAndTargets);
            var keysToMeleeTargets = GetKeysToTargets(choicesAndTargets, SampleGame.Vals.CombatCategory.MeleeTarget);
            var keysToRangedTargets = GetKeysToTargets(choicesAndTargets, SampleGame.Vals.CombatCategory.RangedTarget);

            return GetHeroAction(keysToActions, keysToMeleeTargets, keysToRangedTargets);
        }

        private static ActionChosen GetHeroAction(List<ConsoleKeyAndChoice> keysToChoices, List<ConsoleKeyToTarget> keysToMeleeTargets, List<ConsoleKeyToTarget> keysToRangedTargets)
        {
            var actionChosen = new ActionChosen();

            string category = SampleGame.Vals.CombatCategory.TopLevelAction;

            //there might be a series of choices before we're done. we'll successively fill in parts of actionChosen as we repeat.
            Console.WriteLine("Choose an action:");
            while(true)
            {
                List<IConsoleMap> possibleChoices2 = (category == SampleGame.Vals.CombatCategory.RangedTarget)
                    ? keysToRangedTargets.ToList<IConsoleMap>()
                    : (category == SampleGame.Vals.CombatCategory.MeleeTarget)
                        ? keysToMeleeTargets.ToList<IConsoleMap>()
                        : keysToChoices.Where(kc => kc.Choice.Category == category).ToList<IConsoleMap>();

                //MWCTODO: really ought to implement backspace option here that works same regardless of context.
                DisplayCurrentChoices(possibleChoices2);

                if (category != SampleGame.Vals.CombatCategory.MeleeTarget && category != SampleGame.Vals.CombatCategory.RangedTarget)
                {
                    var choice = (ConsoleKeyAndChoice)GetChoiceFromConsoleKeymap(possibleChoices2);

                    actionChosen.AgentId = choice.Choice.AgentId;
                    actionChosen.Action = choice.Choice.Action;
                    actionChosen.WeaponEntityId = choice.Choice.WeaponEntityId;
                    actionChosen.WeaponHandIndex = choice.Choice.WeaponHandIndex;

                    category = choice.Choice.NextCategory;
                }
                else
                {
                    var choice = (ConsoleKeyToTarget)GetChoiceFromConsoleKeymap(possibleChoices2);

                    actionChosen.TargetEntityId = choice.TargetId;

                    category = null;
                }

                if (category == null) return actionChosen;
            }
        }

        private static void DisplayCurrentChoices<T>(List<T> choices) where T : IConsoleMap
        {
            foreach (var choice in choices)
            {
                string choiceKeyString = GetChoiceKeyString(choice.Cki);
                Console.WriteLine($"{choiceKeyString.PadRight(10)} : {choice.DisplayText}");
            }
        }

        /// <summary>
        /// Returns null if backspace was pressed.
        /// </summary>
        private static T GetChoiceFromConsoleKeymap<T>(List<T> consoleKeyMap) where T : IConsoleMap
        {
            ConsoleKeyInfo cki;
            while(true)
            {
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Backspace) return default(T);

                var match = consoleKeyMap.FirstOrDefault(c => c.Cki == cki);

                if (match != null) return match;
            }
        }

        private static string GetChoiceKeyString(ConsoleKeyInfo cki)
        {
            var parts = new List<string>();

            if (cki.Modifiers == ConsoleModifiers.Control) parts.Add("ctrl");
            if (cki.Modifiers == ConsoleModifiers.Alt) parts.Add("alt");

            parts.Add(cki.Key.ToString().ToUpper());

            return string.Join("-", parts);
        }

        private static List<ConsoleKeyToTarget> GetKeysToTargets(CombatChoicesAndTargets choicesAndTargets, string meleeOrRanged)
        {
            long agentId = choicesAndTargets.Choices.Select(c => c.AgentId).Distinct().Single();

            if (!_agentToTargetMappings.ContainsKey(agentId)) _agentToTargetMappings[agentId] = new Dictionary<long, ConsoleKeyToTarget>();

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

                    var newCki = new ConsoleKeyToTarget { Cki = nextOpenConsoleKey, TargetId = kvp.Key, TargetDescription = kvp.Value };
                    ckiToTarget.Add(newCki);
                    establishedTargets.Add(kvp.Key, newCki);
                }
            }

            return ckiToTarget;
        }

        //oh god this is terrible, but i just want to move on from it for now.
        //  we need keys to be unique FOR THE SAME GROUP of actions, but they don't need to be unique globally.
        static List<ConsoleKeyAndChoice> GetKeysToActions(CombatChoicesAndTargets choicesAndTargets)
        {
            var keysAndChoices = new List<ConsoleKeyAndChoice>();

            //for now (probably always) we support only a single agent at a time.
            long agentId = choicesAndTargets.Choices.Select(c => c.AgentId).Distinct().Single();

            if (!_agentToActionMappings.ContainsKey(agentId)) _agentToActionMappings[agentId] = new List<ConsoleKeyAndChoice>();

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
                keysAndChoices.Add(newCkiAndChoice);
            }

            return keysAndChoices;
        }

        private static ConsoleKeyInfo GetNextOpenConsoleKey(HashSet<char> presetChars, HashSet<char> sameLevelChars)
        {
            for (int i = 97; i <= 122; i++)
            {
                char letter = Convert.ToChar(i);

                if (presetChars.Contains(letter)) continue;
                if (sameLevelChars.Contains(letter)) continue;

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
    }

    interface IConsoleMap
    {
        ConsoleKeyInfo Cki { get; set; }
        string DisplayText { get; }
    }

    public class ConsoleKeyAndChoice : IConsoleMap
    {
        public ConsoleKeyInfo Cki { get; set; }
        public AgentActionChoice Choice { get; set; }

        public string DisplayText
        {
            get { return Choice.Description; }
        }

    }

    public class ConsoleKeyToTarget : IConsoleMap
    {
        public ConsoleKeyInfo Cki { get; set; }
        public long TargetId { get; set; }
        public string TargetDescription { get; set; }

        public string DisplayText
        {
            get { return TargetDescription; }
        }
    }


}