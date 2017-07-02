using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    public class Anatomy : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// Human, quadruped, avian, insect, etc. Defines which equipment slots are available for an anatomy.
        /// </summary>
        public string BodyPlan { get; set; }

        /// <summary>
        /// Says which slots are normally used for attacks. Expected to be a subset of body plan (e.g. human, wolf, horse),
        /// although weird anatomies might be weird in this regard. An empty/null string here means no natural weapons.
        /// </summary>
        public string NaturalWeaponsCategory { get; set; }

        /// <summary>
        /// Equipped includes wielding. The Value is the entityId of the weapon that is being equipped. 
        /// A Value of 0 means nothing is equipped in that slot. See Equipment system for details.
        /// </summary>
        public Dictionary<Vals.BodySlots, long> SlotsEquipped { get; set; } = new Dictionary<Vals.BodySlots, long>();
    }
}
