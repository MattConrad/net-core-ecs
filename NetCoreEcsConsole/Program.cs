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
            var nextHeroAction = GetHeroAction(actionInputSet);
            Console.WriteLine(nextHeroAction);

            var lines = game.ProcessInput("nothing");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        static Dictionary<char, TextActionPair> GetKeysToActions(Dictionary<string, string> heroActionDict)
        {
            Dictionary<char, TextActionPair> keyToTAP = new Dictionary<char, TextActionPair>();

            int i = 49;
            foreach (string action in heroActionDict.Keys)
            {
                string text = heroActionDict[action];
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
                cki = Console.ReadKey();
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