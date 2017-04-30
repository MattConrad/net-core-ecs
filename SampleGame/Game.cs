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

            long weaponId = WeaponCreator.Weapon(this.Rgs, "sword", "sword", Parts.SingleTargetDamager.Vals.DamageType.MechanicalSlashing, 2000);
            long armorId = ArmorCreator.Armor(this.Rgs, "leather armor", "leather", 1.0m, 1000);

            //for right now, both combatants will magically share the same equipment . . .
            var equipmentIds = new[] { weaponId, armorId };
            //this.HeroId = AgentCreator.Agent(this.Rgs, "The Hero", "person", "The hero, who must emerge from battle victorious.", equipmentIds, "pack", 3, Parts.Faction.Vals.FactionName.Heroes);
            //this.VillainId = AgentCreator.Agent(this.Rgs, "Gruk", "orc", "A nasty orc who must surely die upon the hero's blade.", equipmentIds, "pack", 2, Parts.Faction.Vals.FactionName.Villians);
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
