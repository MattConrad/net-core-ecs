using EntropyEcsCore;

namespace SampleGame
{
    public static class ArmorCreator
    {
        internal static long Armor(EcsRegistrar rgs, string generalName, string shortDescription, decimal defaultDamageMultiplier, int defaultDamageThreshold)
        {
            long armorId = rgs.CreateEntity();

            rgs.AddPart(armorId, new Parts.PhysicalObject());
            rgs.AddPart(armorId, new Parts.EntityName { ProperName = "", GeneralName = generalName, ShortDescription = shortDescription });
            rgs.AddPart(armorId, new Parts.DamagePreventer { DamageMultiplier = defaultDamageMultiplier, DamageThreshold = defaultDamageThreshold });

            return armorId;
        }

    }
}
