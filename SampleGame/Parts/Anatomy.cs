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
        /// Equipped includes wielding. The Value is the entityId of the weapon that is being equipped. 
        /// A Value of 0 means nothing is equipped in that slot. See Equipment system for details.
        /// </summary>
        public Dictionary<Vals.BodySlots, long> SlotsEquipped { get; set; } = new Dictionary<Vals.BodySlots, long>();
    }
}
