using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    public class Game
    {
        private EcsRegistrar Rgs { get; set; } = new EcsRegistrar();

        public List<string> ProcessInput(string input)
        {
            return new List<string> { "these", "are", "lines" };
        }
    }
}
