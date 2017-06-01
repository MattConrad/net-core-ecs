using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    /// <summary>
    /// The equipment system also handles wielding.
    /// </summary>
    internal static class Equipment
    {
        internal enum WeaponHandsRequired
        {
            WeaponTooSmall = 0,
            One = 1,
            Two = 2,
            WeaponTooBig = 3
        }

        //the default wielder is human, and the default weapon-object takes two hands for a human to wield.
        // there is no real good default for object wieldability so this can go wrong easily.
        internal static int _defaultWielderSizeInt = Vals.Size.SizeToInt[Vals.Size.Human];
        internal static int _defaultWeaponWieldSizeInt = Vals.Size.SizeToInt[Vals.Size.Goliath];

        internal static Output Equip(EcsRegistrar rgs, long equipperId, long gearId, bool autoUnequipExisting)
        {
            var output = new Output { Category = "Text" };

            var equipperAnatomy = rgs.GetPartSingle<Parts.Anatomy>(equipperId);
            var equipperAnatomyModifications = rgs.GetParts<Parts.AnatomyModifier>(equipperId).ToList();
            var equipperPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(equipperId);
            var equipperEntityName = rgs.GetPartSingle<Parts.EntityName>(equipperId);
            var gearPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(gearId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            var equipperActiveSlots = (Vals.BodyPlan.BodyPlanEquipmentSlots.GetValueOrDefault(equipperAnatomy.BodyPlan, null) ?? new string[] { }).ToList();
            var equipperBlockedSlots = equipperAnatomyModifications.Select(m => m.BodyPlanEquipmentSlotDisabled).ToList();
            foreach (var blocked in equipperBlockedSlots)
            {
                equipperActiveSlots.Remove(blocked);
            };

            //someday we'll support gear that occupies multiple slots or varying slots.
            if (gearPhysicalObject.EquipmentSlot == Vals.BodyParts.Special) throw new NotImplementedException();

            if (!equipperActiveSlots.Contains(gearPhysicalObject.EquipmentSlot))
            {
                //MWCTODO: maybe this should be more descriptive. e.g. "doesn't have a head!"
                output.Data = $"The {gearEntityName.GeneralName} doesn't fit {equipperEntityName.ProperName}'s anatomy.";

                return output;
            }

            foreach (string equipped in equipperAnatomy.SlotsEquipped.Keys)
            {
                equipperActiveSlots.Remove(equipped);
            }

            if (!equipperActiveSlots.Contains(gearPhysicalObject.EquipmentSlot))
            {
                output.Data = $"The {gearEntityName.GeneralName} already has gear equipped there. Remove something else first.";

                return output;
            }

            equipperAnatomy.SlotsEquipped.Add(gearPhysicalObject.EquipmentSlot, gearId);
            output.Data = $"{equipperEntityName.ProperName} puts on the {gearEntityName.GeneralName}.";

            return output;
        }

        // MWCTODO: maybe this could be one method for all equips . . . finish wielding first, then think about this.
        /// <summary>
        /// Equipping a shield is really wielding one, though we may present it as equipping to the user. Yes, a tangle.
        /// </summary>
        internal static Output WieldWeapon(EcsRegistrar rgs, long wielderId, long weaponId)
        {
            var output = new Output { Category = "Text" };

            var wielderAnatomy = rgs.GetPartSingle<Parts.Anatomy>(wielderId);
            var wielderAnatomyModifications = rgs.GetParts<Parts.AnatomyModifier>(wielderId).ToList();
            var wielderPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(wielderId);
            var wielderEntityName = rgs.GetPartSingle<Parts.EntityName>(wielderId);
            var weaponPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(weaponId);
            var weaponEntityName = rgs.GetPartSingle<Parts.EntityName>(weaponId);

            var handsRequired = GetWeaponHandsRequired(wielderPhysicalObject, weaponPhysicalObject);

            //MWCTODO: "to wield", what if it's a shield? (see remarks below about main/off hand auto-determination--we're not going to do that).
            if (handsRequired == WeaponHandsRequired.WeaponTooBig) output.Data = $"The {weaponEntityName.GeneralName} is too big for {wielderEntityName.Pronoun} to wield.";
            if (handsRequired == WeaponHandsRequired.WeaponTooSmall) output.Data = $"The {weaponEntityName.GeneralName} is too small for {wielderEntityName.Pronoun} to wield.";

            if (handsRequired == WeaponHandsRequired.WeaponTooBig || handsRequired == WeaponHandsRequired.WeaponTooSmall)
            {
                return output;
            }

            var wielderActiveHandSlots = (Vals.BodyPlan.BodyPlanEquipmentSlots.GetValueOrDefault(wielderAnatomy.BodyPlan, null) ?? new string[] { })
                .Where(s => s == Vals.BodyParts.WieldObjectAppendage)
                .ToList();
            var wielderBlockedHandSlots = wielderAnatomyModifications
                .Select(m => m.BodyPlanEquipmentSlotDisabled)
                .Where(s => s == Vals.BodyParts.WieldObjectAppendage)
                .ToList();
            //body plans have lots of duplicate entries, so can't use set-based .Except().
            foreach (var blocked in wielderBlockedHandSlots)
            {
                wielderActiveHandSlots.Remove(blocked);
            };

            if ((int)handsRequired > wielderActiveHandSlots.Count())
            {
                var start = $"The {weaponEntityName.GeneralName} requires {(int)handsRequired} to wield, and {wielderEntityName.Pronoun} ";
                var finish = wielderActiveHandSlots.Count > 0 ? $"has only {wielderActiveHandSlots.Count}." : "doesn't have any!";
                output.Data = start + finish;

                return output;
            }

            foreach(string equipped in wielderAnatomy.SlotsEquipped.Keys.Where(k => k == Vals.BodyParts.WieldObjectAppendage))
            {
                wielderActiveHandSlots.Remove(equipped);
            }

            ////for now, we will assume the wielder always wants to equip in the primary hand slot--but this is only a temporary simplification.
            if (wielderActiveHandSlots.Count >= (int)handsRequired)
            {
                if (handsRequired == WeaponHandsRequired.Two)
                {
                    wielderAnatomy.SlotsEquipped[wielderActiveHandSlots[0]] = weaponId;
                    wielderAnatomy.SlotsEquipped[wielderActiveHandSlots[1]] = weaponId;
                    output.Data = $"{wielderEntityName.ProperName} grips the {weaponEntityName.GeneralName} firmly in two hands.";
                }
                else if (handsRequired == WeaponHandsRequired.One)
                {
                    //had been trying to automatically do main/off hand swapping here. this was a mistake. 
                    // if you make swapping hands a free action this means the whole main/off thing becomes much simpler.
                    wielderAnatomy.SlotsEquipped[wielderActiveHandSlots[0]] = weaponId;
                    output.Data = $"{wielderEntityName.ProperName} wields the {weaponEntityName.GeneralName}.";
                }
            }
            else
            {
                output.Data = $"{wielderEntityName.ProperName} already has {wielderEntityName.Pronoun}[poss-adj] hands full. Unequip something first.";
            }

            return output;
        }

        /// <summary>
        /// Hands required to wield a weapon/object.
        /// Weapons matching or one step smaller than the wielder can be wielded one handed.
        /// Weapons one step bigger than the wielder can be wielded 2 handed.
        /// Weapons outside those ranges cannot be wielded.
        /// </summary>
        internal static WeaponHandsRequired GetWeaponHandsRequired(Parts.PhysicalObject wielderPhysicalObject, Parts.PhysicalObject weaponPhysicalObject)
        {
            int wielderSizeInt = Vals.Size.SizeToInt.GetValueOrDefault(wielderPhysicalObject.Size ?? "", _defaultWielderSizeInt);
            int weaponWieldSizeInt = Vals.Size.SizeToInt.GetValueOrDefault(weaponPhysicalObject.Size ?? "", _defaultWeaponWieldSizeInt);

            int delta = weaponWieldSizeInt - wielderSizeInt;

            if (delta < -1) return WeaponHandsRequired.WeaponTooSmall;

            if (delta == -1 || delta == 0) return WeaponHandsRequired.One;

            if (delta == 1) return WeaponHandsRequired.Two;

            return WeaponHandsRequired.WeaponTooBig;
        }

    }
}