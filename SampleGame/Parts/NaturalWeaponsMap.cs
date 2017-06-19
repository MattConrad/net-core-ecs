using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    public class NaturalWeaponsMap : EntropyEcsCore.EcsEntityPart
    {
        public Dictionary<string, long> NameToNaturalWeaponId { get; set; }
    }
}
