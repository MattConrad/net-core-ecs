﻿using System.Collections.Generic;
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

            this.HeroId = AgentCreator.Agent(this.Rgs, "agent.hero", "obj.armor.leather-armor", "obj.weapon.sword");
            this.VillainId = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
        }

        public List<string> ProcessInput(string input)
        {
            List<string> results = new List<string>();
            bool combatFinished = false;
            while (!combatFinished)
            {
                var actionResults = Sys.Combat.HerosAction(this.Rgs, this.HeroId, this.VillainId, out combatFinished);
                results.AddRange(actionResults);
            }

            return results;
        }
    }
}
