using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    public class Anatomy : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// Humanoid, quadruped, avian, insect, etc.
        /// </summary>
        public string BodyPlan { get; set; }

        /// <summary>
        /// Equipped includes wielding, but this does get a little complicated. See Equipment system.
        /// </summary>
        public Dictionary<string, long> SlotsEquipped { get; set; } = new Dictionary<string, long>();
    }
}
