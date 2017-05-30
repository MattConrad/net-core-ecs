using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    /// <summary>
    /// Certain physical objects are meant to be equippable in a body slot (like armor, rings, necklaces).
    /// </summary>
    public class Equippable : EntropyEcsCore.EcsEntityPart
    {
        //normally this will be a single equipment slot, but some items might require several slots at once.
        public List<string> EquipmentSlots { get; set; } = new List<string>();


    }
}
