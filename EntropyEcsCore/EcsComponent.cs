using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntropyEcsCore
{
    /// <summary>
    /// The Type of a component indicates what kind of Data is found for that component.
    /// </summary>
    public class EcsComponent
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public DataDict Data { get; set; } = new DataDict();
    }
}
