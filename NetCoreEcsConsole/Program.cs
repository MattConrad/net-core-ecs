using System;
using System.Collections.Generic;
using SampleGame;

namespace NetCoreEcsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var actionKeyboardMappings = GetInitialActionKeyboardMappings();
            var targetKeyboardMappings = new Dictionary<char, Dictionary<char, ActionChosen>>();

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

        private static Dictionary<char, string> GetInitialActionKeyboardMappings()
        {
            var map = new Dictionary<char, string>();

            map.Add('a', SampleGame.Vals.CombatAction.AttackWeaponMelee);
            map.Add('f', SampleGame.Vals.CombatAction.AttackWeaponRanged);
            map.Add('c', SampleGame.Vals.CombatAction.CastSpell);
            map.Add('z', SampleGame.Vals.CombatAction.SwitchToAI);

            return map;
        }

        public static string SendPlayerInput(Dictionary<string, string> heroActionDict)
        {
            var actionInputSet = GetKeysToActions(heroActionDict);
            return GetHeroAction(actionInputSet);
        }

        public static ActionChosen SendPlayerInput2(Dictionary<string, string> heroActionDict)
        {
            //var actionInputSet = GetKeysToActions2(heroActionDict);
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
}