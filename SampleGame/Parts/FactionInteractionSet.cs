using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    public class FactionInteractionSet : EntropyEcsCore.EcsEntityPart
    {
        public List<FactionInteraction> Interactions { get; set; } = new List<FactionInteraction>();
    }

    public class FactionInteraction
    {
        public string SourceFaction { get; set; }
        public string TargetFaction { get; set; }
        public int Disposition { get; set; }
    }

}
