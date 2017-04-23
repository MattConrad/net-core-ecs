using System;
using System.Collections.Generic;
using System.Text;
using EntropyEcsCore;

namespace SampleGame
{
    public static class ArmorCreator
    {
        internal static long Armor(EcsRegistrar rgs, string generalName, string shortDescription, long defenseValue)
        {
            long armorId = rgs.CreateEntity();

            rgs.AddComponent(armorId, new Parts.PhysicalObject());
            rgs.AddComponent(armorId, new Parts.EntityName { ProperName = "", GeneralName = generalName, ShortDescription = shortDescription });

            return armorId;
        }

    }
}
