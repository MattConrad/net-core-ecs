﻿using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    /// <summary>
    /// The anatomy system also handles equipping items.
    /// </summary>
    internal static class Anatomy
    {
        internal static Output Unequip(EcsRegistrar rgs, long unequipperId, long gearId)
        {
            var output = new Output { Category = "Text" };

            var unequipperEntityName = rgs.GetPartSingle<Parts.EntityName>(unequipperId);
            var unequipperAnatomy = rgs.GetPartSingle<Parts.Anatomy>(unequipperId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            bool unequipped = false;
            foreach(var slot in unequipperAnatomy.SlotsEquipped)
            {
                if (slot.Value == gearId)
                {
                    unequipperAnatomy.SlotsEquipped[slot.Key] = 0L;
                    unequipped = true;
                }
            }

            output.Data = unequipped
                ? output.Data = $"{unequipperEntityName.ProperName} removes the {gearEntityName.GeneralName}."
                : output.Data = $"{unequipperEntityName.ProperName} doesn't have the {gearEntityName.GeneralName} equipped and can't remove it.";

            return output;
        }

        internal static Output Equip(EcsRegistrar rgs, long equipperId, long gearId)
        {
            var output = new Output { Category = "Text" };

            //MWCTODO: bundleize this

            var equipperAnatomy = rgs.GetPartSingle<Parts.Anatomy>(equipperId);
            var equipperPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(equipperId);
            var equipperEntityName = rgs.GetPartSingle<Parts.EntityName>(equipperId);
            var gearPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(gearId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            //someday we'll support gear that occupies multiple slots or using complex slot logic.
            if (gearPhysicalObject.EquipmentSlots == Vals.BodySlots.NoSlot || gearPhysicalObject.EquipmentSlots == Vals.BodySlots.Special) throw new NotImplementedException();

            //MWCTODO++: rework this to use new DisabledSlot entity concept.
            //var equipperDisabledSlots = equipperAnatomyModifications.Select(m => m.NewEquipmentSlotsDisabled).ToList();
            var equipperSlots = equipperAnatomy
                .SlotsEquipped
                //.Where(kvp => !equipperDisabledSlots.Contains(kvp.Key))
                .ToDictionary(d => d.Key, d => d.Value);

            var matchingEquipperSlots = equipperSlots.Where(slot => (slot.Key & gearPhysicalObject.EquipmentSlots) == slot.Key).ToList();
            if (!matchingEquipperSlots.Any())
            {
                output.Data = $"{equipperEntityName.ProperName} doesn't have the right body parts to equip the {gearEntityName.GeneralName}.";
                return output;
            }

            if (matchingEquipperSlots.All(s => s.Value != 0) )
            {
                output.Data = $"There's no open place to equip the {gearEntityName.GeneralName}. Unequip something else first.";
                return output;
            }

            //we will special case 2h weapons here, though. and body armor!
            bool gearIs2Handed = (gearPhysicalObject.EquipmentSlots & Vals.BodySlots.WieldTwoHanded) == Vals.BodySlots.WieldTwoHanded;
            bool gearIsHumanBodyArmor = (gearPhysicalObject.EquipmentSlots & Vals.BodySlots.HumanBodyArmor) == Vals.BodySlots.HumanBodyArmor;

            if (gearIs2Handed && Vals.BodyPlan.BodySlotsEncompassedByOtherSlot[Vals.BodySlots.WieldTwoHanded].Any(s => equipperAnatomy.SlotsEquipped[s] != 0))
            {
                output.Data = $"Two free hands are needed for the {gearEntityName.GeneralName}. Unequip something else first.";
                return output;
            }

            if (gearIsHumanBodyArmor && Vals.BodyPlan.BodySlotsEncompassedByOtherSlot[Vals.BodySlots.HumanBodyArmor].Any(s => equipperAnatomy.SlotsEquipped[s] != 0))
            {
                output.Data = $"Other equipment is in the way of equipping the {gearEntityName.GeneralName}. Unequip other gear first.";
                return output;
            }

            //by equipping 2h-weapons and body armor in ALL the related slots, it makes checking equipment/weapons easy elsewhere. it will complicate unequip, though.

            var firstOpenSlot = matchingEquipperSlots.First(s => s.Value == 0);
            equipperAnatomy.SlotsEquipped[firstOpenSlot.Key] = gearId;
            if (gearIs2Handed)
            {
                foreach (var slot in Vals.BodyPlan.BodySlotsEncompassedByOtherSlot[Vals.BodySlots.WieldTwoHanded])
                {
                    equipperAnatomy.SlotsEquipped[slot] = gearId;
                }
            }

            if (gearIsHumanBodyArmor)
            {
                foreach(var slot in Vals.BodyPlan.BodySlotsEncompassedByOtherSlot[Vals.BodySlots.HumanBodyArmor])
                {
                    equipperAnatomy.SlotsEquipped[slot] = gearId;
                }
            }

            output.Data = $"{equipperEntityName.ProperName} equips the {gearEntityName.GeneralName}.";
            return output;
        }

        //for now, hardcoding this is fine. once we get a dozen, reconsider (or maybe not).
        internal static Dictionary<string, Dictionary<Vals.BodySlots, long>> InitializeNaturalWeapons(EcsRegistrar rgs)
        {
            var humanPunchId = Blueprinter.GetEntityFromBlueprint(rgs, Vals.NaturalWeaponNames.HumanPunch);
            var humanNatWeapons = new Dictionary<Vals.BodySlots, long>
            {
                [Vals.BodySlots.WieldHandLeft] = humanPunchId,
                [Vals.BodySlots.WieldHandRight] = humanPunchId
            };

            var wolfBiteId = Blueprinter.GetEntityFromBlueprint(rgs, Vals.NaturalWeaponNames.WolfBite);
            var wolfNatWeapons = new Dictionary<Vals.BodySlots, long>
            {
                [Vals.BodySlots.VertebrateMouth] = wolfBiteId
            };

            return new Dictionary<string, Dictionary<Vals.BodySlots, long>>
            {
                [Vals.NaturalWeaponSet.Human] = humanNatWeapons,
                [Vals.NaturalWeaponSet.Wolf] = wolfNatWeapons
            };
        }

    }
}