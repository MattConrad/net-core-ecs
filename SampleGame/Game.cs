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
            public string Attack = nameof(Attack);
            public string RunAway = nameof(RunAway);
            public string Examine = nameof(Examine);
        }

        private EcsRegistrar Rgs { get; set; }
        //hero is what we call the player's entity. we always know this for single-player context.
        //  if multiplayer, "hero" ids would be different for each player and passed in.
        private long HeroId { get; set; }
        //we'll have arbitrary numbers of villains real soon now.
        private long VillainId { get; set; }

        public Game()
        {
            this.Rgs = new EcsRegistrar();
            this.HeroId = AgentCreator.Agent(this.Rgs, "The Hero", "person", "The hero, who must emerge from battle victorious.", null, "pack", Parts.Faction.Vals.FactionName.Heroes);
            this.VillainId = AgentCreator.Agent(this.Rgs, "Gruk", "orc", "A nasty orc who must surely die upon the hero's blade.", null, "pack", Parts.Faction.Vals.FactionName.Villians);
        }

        public List<string> ProcessInput(string input)
        {
            List<string> results = new List<string>();
            bool combatFinished = false;
            while (!combatFinished)
            {
                var actionResults = CombatSystem.HerosAction(this.Rgs, this.HeroId, this.VillainId, out combatFinished);
                results.AddRange(actionResults);
            }

            return results;
        }
    }
}
