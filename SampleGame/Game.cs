using System;
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

        private Func<CombatChoicesAndTargets, ActionChosen> ReceivePlayerInputFunc { get; set; }
        private EcsRegistrar Rgs { get; set; }

        private long GlobalId { get; set; }
        private long BattlefieldId { get; set; }

        public Game(Func<CombatChoicesAndTargets, ActionChosen> receivePlayerInputFunc)
        {
            this.ReceivePlayerInputFunc = receivePlayerInputFunc;
            this.Rgs = new EcsRegistrar();

            this.GlobalId = Blueprinter.GetEntityFromBlueprint(this.Rgs, "global");

            long heroId = AgentCreator.Agent(this.Rgs, "agent.hero", "obj.armor.chainmail", "obj.weapon.claymore");
            long villian1Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian2Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian3Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            Sys.EntityName.Overwrite(this.Rgs, villian1Id, new Parts.EntityName { ProperName = "Gruk" });
            Sys.EntityName.Overwrite(this.Rgs, villian2Id, new Parts.EntityName { ProperName = "Hork" });
            Sys.EntityName.Overwrite(this.Rgs, villian3Id, new Parts.EntityName { ProperName = "Lil Jon" });

            this.BattlefieldId = Sys.Container.CreateBattlefield(this.Rgs, new long[] { heroId, villian1Id, villian2Id, villian3Id });
        }

        public IEnumerable<List<Output>> RunGame()
        {
            //for now, running the game and running the battlefield are the same thing.
            foreach(var outputs in Sys.Battlefield.RunBattlefield(this.Rgs, this.GlobalId, this.BattlefieldId, this.ReceivePlayerInputFunc))
            {
                yield return outputs;
            }
        }
    }
}
