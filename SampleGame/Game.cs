using System.Collections.Generic;
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

            //MWCTODO: ummm, Gruk (and presumably hero) actually have two weapons, not a weapona and an armor....oh, crap, blueprint is broken. doh.
            this.HeroId = AgentCreator.Agent(this.Rgs, "agent.hero", "obj.armor.leather-armor", "obj.weapon.sword");
            this.VillainId = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
        }

        public List<string> ProcessInput(string heroAction, out bool combatFinished)
        {
            List<string> results = new List<string>();

            var actionResults = Sys.Combat.HerosAction(this.Rgs, this.HeroId, this.VillainId, heroAction, out combatFinished);
            results.AddRange(actionResults);

            return results;
        }
    }
}
