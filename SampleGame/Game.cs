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

        private long BattlefieldId { get; set; }

        public Game(Func<Dictionary<string, string>, string> receivePlayerInputFunc)
        {
            this.ReceivePlayerInputFunc = receivePlayerInputFunc;
            this.Rgs = new EcsRegistrar();

            long heroId = AgentCreator.Agent(this.Rgs, "agent.hero", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian1Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            long villian2Id = AgentCreator.Agent(this.Rgs, "agent.monster.orc.basic", "obj.armor.leather-armor", "obj.weapon.sword");
            Sys.EntityName.Overwrite(this.Rgs, villian1Id, new Parts.EntityName { ProperName = "Gruk" });
            Sys.EntityName.Overwrite(this.Rgs, villian2Id, new Parts.EntityName { ProperName = "Hork" });

            this.BattlefieldId = Sys.Container.CreateBattlefield(this.Rgs, new long[] { heroId, villian1Id, villian2Id });
        }

        public IEnumerable<List<Output>> RunGame()
        {
            //for now, running the game and running the battlefield are the same thing.
            foreach(var outputs in Sys.Battlefield.RunBattlefield(this.Rgs, this.BattlefieldId, this.ReceivePlayerInputFunc))
            {
                yield return outputs;
            }
        }
    }
}
