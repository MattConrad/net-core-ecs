using System;
using System.Collections.Generic;
using System.Linq;
using SampleGame;

namespace NetCoreEcsConsole
{
    class Program
    {
        private static Dictionary<long, List<ChoiceDisplay>> _agentToActionMappings = new Dictionary<long, List<ChoiceDisplay>>();

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
            var actionInputSet = GetKeysToActions2(choicesAndTargets);
            throw new NotImplementedException();
            //return GetHeroAction2(actionInputSet);
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

        static List<ChoiceDisplay> GetKeysToActions2(CombatChoicesAndTargets choicesAndTargets)
        {
            var choicesToDisplay = new List<ChoiceDisplay>();

            //for now (probably always) we support only a single agent at a time.
            long agentId = choicesAndTargets.Choices.Select(c => c.AgentId).Distinct().Single();
            var establishedChoices = _agentToActionMappings[agentId];

            foreach(var choice in choicesAndTargets.Choices)
            {
                ChoiceDisplay presetChoiceDisplay = GetPreset(choice);
                if (presetChoiceDisplay != null)
                {
                    choicesToDisplay.Add(presetChoiceDisplay);
                    continue;
                }

                ChoiceDisplay establishedChoiceDisplay = GetEstablishedChoice(establishedChoices, choice);
                if (establishedChoiceDisplay != null)
                {
                    choicesToDisplay.Add(establishedChoiceDisplay);
                    continue;
                }

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
            }

            return choicesToDisplay;
        }

        private static ChoiceDisplay GetEstablishedChoice(List<ChoiceDisplay> establishedChoices, AgentActionChoice choices)
        {


        }

        private static ChoiceDisplay GetPreset(AgentActionChoice choice)
        {
            //only top level actions will get preset keys. 

            throw new NotImplementedException();
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

    public class ChoiceDisplay
    {
        public ConsoleKeyInfo Cki { get; set; }
        public AgentActionChoice Choice { get; set; }
    }


}