using EntropyEcsCore;

namespace SampleGame
{
    public static class ArmorCreator
    {
        internal static long Armor(EcsRegistrar rgs, string generalName, string shortDescription, decimal defaultDamageMultiplier, int defaultDamageThreshold)
        {
            long armorId = rgs.CreateEntity();

            rgs.AddComponent(armorId, new Parts.PhysicalObject());
            rgs.AddComponent(armorId, new Parts.EntityName { ProperName = "", GeneralName = generalName, ShortDescription = shortDescription });
            rgs.AddComponent(armorId, new Parts.DamagePreventer { DamageMultiplier = defaultDamageMultiplier, DamageThreshold = defaultDamageThreshold });

            return armorId;
        }

    }
}
