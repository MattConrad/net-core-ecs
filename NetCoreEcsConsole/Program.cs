using System;
using System.Collections.Generic;
using SampleGame;

namespace NetCoreEcsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            var heroActionDict = Switchboard.HerosActions();
            var actionInputSet = GetKeysToActions(heroActionDict);
            //this might not even happen until the game has started and presented the player with an initial scenario.

            bool combatFinished = false;
            while(!combatFinished)
            {
                var nextHeroAction = GetHeroAction(actionInputSet);
                Console.WriteLine(nextHeroAction);

                var lines = game.ProcessInput(nextHeroAction, out combatFinished);
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
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