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
        public string EquipmentSlot { get; set; }


    }
}
