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
        /// Equipped includes wielding. The KVP Key is the name of the slot that is equipped, the Value is the entityId of the 
        /// weapon that is being equipped. A Value of 0 means nothing is equipped in that slot. See Equipment system for details.
        /// </summary>
        public List<KeyValuePair<string, long>> SlotsEquipped { get; set; } = new List<KeyValuePair<string, long>>();

        /// <summary>
        /// Equipped includes wielding. The Value is the entityId of the weapon that is being equipped. 
        /// A Value of 0 means nothing is equipped in that slot. See Equipment system for details.
        /// </summary>
        public Dictionary<Vals.BodySlots, long> NewSlotsEquipped { get; set; } = new Dictionary<Vals.BodySlots, long>();
    }
}
