﻿//using EntropyEcsCore;

//namespace SampleGame
//{
//    public static class WeaponCreator
//    {
//        internal static long Weapon(EcsRegistrar rgs, string generalName, string shortDescription, string damageType, int damageAmount)
//        {
//            long weaponId = rgs.CreateEntity();

//            rgs.AddPart(weaponId, new Parts.PhysicalObject());
//            rgs.AddPart(weaponId, new Parts.EntityName { ProperName = "", GeneralName = generalName, ShortDescription = shortDescription });
//            rgs.AddPart(weaponId, new Parts.SingleTargetDamager { DamageType = damageType, DamageAmount = damageAmount });

//            return weaponId;
//        }
//    }
//}
