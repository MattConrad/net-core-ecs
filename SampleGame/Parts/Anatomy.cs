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

        /* So, List<KVP> is kind of weird, right?
         * 
         * For a while this was a dictionary, but we don't really want unique keys: there are lots of repeated anatomy parts,
         * notably you might want to have two wielding hands, but also two of lots of other things, and I also wanted to 
         * easily (cough cough) make non standard anatomies like formicids or octopodes or nagas. If using a dictionary
         * each of those parts has to have a unique name, and then if you want to equip a ring then we have to check 
         * both RigntRingFinger and LeftRingFinger individually. Ugh.
         * 
         * Using List<KVP> allow anatomy to have [RingFinger, RingFinger] in the anatomy of an entity, each one as a distinct
         * slot, and both working the same way as far as what gear fits in that slot.
         * 
         * However, I haven't pushed on this very far yet. I suspect when I get to anatomy modifications this is going to get
         * a little interesting, because (see Vals.Anatomy) we have multiple slots that refer to the same body part. For example
         * WieldObjectAppendage, HumanoidGauntletHandPair, HumanoidRingFinger, HumanoidArm are all related and you should lose
         * all of them if you get your arm chopped off. Right now there is no awareness of these sorts of relationships.
         * I don't have a plan here either, only some hazy ideas.
         * 
         */
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
