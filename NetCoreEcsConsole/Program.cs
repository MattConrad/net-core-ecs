using System;
using SampleGame;

namespace NetCoreEcsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();

            var lines = game.ProcessInput("nothing");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}