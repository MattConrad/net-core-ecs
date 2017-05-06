using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    public class SkillsModifier : EntropyEcsCore.EcsEntityPart
    {
        public long Expiration { get; set; }

        public string Tag { get; set; }

        public Dictionary<string, int> SkillDeltas { get; set; } = new Dictionary<string, int>();
    }
}
