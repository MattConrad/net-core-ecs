using System;
using System.Linq;
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

        private Func<Dictionary<string, string>, string> ReceivePlayerInputFunc { get; set; }
        private EcsRegistrar Rgs { get; set; }

        //hero is what we call the player's entity. we always know this for single-player context.
        //  if multiplayer, "hero" ids would be different for each player and passed in.
        private long HeroId { get; set; }
        //MWCTODO: these go away entirely, we only know about villians via the battlefield. probably this is also true of the hero, at least w/in a single combat.
        private List<long> VillainIds { get; set; } = new List<long>();

        private long BattlefieldId { get; set; }

        public Game(Func<Dictionary<string, string>, string> receivePlayerInputFunc)
        {
            this.ReceivePlayerInputFunc = receivePlayerInputFunc;
            this.Rgs = new EcsRegistrar();

            this.HeroId = AgentCreator.Agent(this.Rgs, "agent.hero", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian1Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian2Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            Sys.EntityName.Overwrite(this.Rgs, villian1Id, new Parts.EntityName { ProperName = "Gruk" });
            Sys.EntityName.Overwrite(this.Rgs, villian2Id, new Parts.EntityName { ProperName = "Hork" });

            this.BattlefieldId = Sys.Container.CreateBattlefield(this.Rgs, new long[] { this.HeroId, villian1Id, villian2Id });

            this.VillainIds.Add(villian1Id);
            this.VillainIds.Add(villian2Id);
        }

        public IEnumerable<List<string>> RunGame()
        {
            //for now, running the game and running the battlefield are the same thing.
            foreach(var lines in Sys.Battlefield.RunBattlefield(this.Rgs, this.BattlefieldId, this.ReceivePlayerInputFunc))
            {
                yield return lines;
            }
        }

        //public List<string> ProcessInput(string heroAction, out bool combatFinished)
        //{
        //    List<string> results = new List<string>();

        //    do
        //    {
        //        //MWCTODO: fix .First() obvs
        //        var actionResults = Sys.Combat.PlayerChoiceAction(this.Rgs, this.HeroId, this.VillainIds.First(), heroAction, out combatFinished);
        //        results.AddRange(actionResults);

        //        //this needs to move back in to the Combat system. we need a battlefield entity.
        //        foreach(long villianId in this.VillainIds)
        //        {
        //            if (!combatFinished) results.AddRange(Sys.Combat.ResolveSingleTargetMelee(this.Rgs, villianId, this.HeroId, out combatFinished));
        //        }

        //    } while (!combatFinished && heroAction == Sys.Combat.Actions.AttackMeleeContinously);

        //    return results;
        //}
    }
}
