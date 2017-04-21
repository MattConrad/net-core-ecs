using System;
using System.Collections.Generic;
using System.Text;

//MWCTODO: this goes away.
namespace NetCoreEcsConsole
{
    public abstract class Cpt
    {
        public long Id { get; set; }
    }

    public class ArmorCpt : Cpt
    {
        public long DefenseValue { get; set; }
        public string Category { get; set; }
    }

    public class ContainerCpt : Cpt
    {
        public List<long> ContainedIds { get; set; } = new List<long>();
    }

}
