using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    public class Game
    {
        public class Actions
        {
            public string Attack = CombatActionMessage.Vals.TargetType.SingleMelee;
            public string RunAway = nameof(RunAway);
        }

        private EcsRegistrar Rgs { get; set; } = new EcsRegistrar();

        public List<string> ProcessInput(string input)
        {
            List<string> results = new List<string>();
            bool continuing = true;
            while (continuing)
            {
                var actionResults = CombatSystem.AttemptAction(this.Rgs, 0L, 0L, out continuing);
                results.AddRange(actionResults);
            }

            return results;
        }
    }
}
