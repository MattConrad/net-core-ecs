using System;
using SampleGame;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetCoreEcsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var game = new Game();

            //var lines = game.ProcessInput("nothing");
            //foreach (string line in lines)
            //{
            //    Console.WriteLine(line);
            //}

            //MWCTODO: remove JSON.NET from here once you're done farting around.

            Dictionary<long, List<Cpt>> entitiesToComponents = new Dictionary<long, List<Cpt>>();

            var cptArmor1 = new ArmorCpt { Id = 11, Category = "leather", DefenseValue = 1000 };
            var cptContain1 = new ContainerCpt { Id = 13, ContainedIds = new List<long> { 20L, 21L, 22L } };

            var cptContain2 = new ContainerCpt { Id = 14, ContainedIds = new List<long> { 30L, 31L, 32L } };
            var cptArmor2 = new ArmorCpt { Id = 12, Category = "chain", DefenseValue = 2000 };

            var cps1 = new List<Cpt> { cptArmor1, cptContain1 };
            var cps2 = new List<Cpt> { cptArmor2, cptContain2 };

            entitiesToComponents.Add(1, cps1);
            entitiesToComponents.Add(2, cps2);

            //this doesn't give enough type information to deserialize; attempting desz crashes.
            // however, all the properties of derived types ARE included in sz output.
            // see http://stackoverflow.com/questions/8513042/json-net-serialize-deserialize-derived-types, looks like > 1 way to address this.

            var sz1 = JsonConvert.SerializeObject(entitiesToComponents);

            var e2c2 = JsonConvert.DeserializeObject<Dictionary<long, List<Cpt>>>(sz1);

        }
    }
}