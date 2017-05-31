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
            var gearEquippable = rgs.GetPartSingle<Parts.Equippable>(gearId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            //this is a little tricky: count two anatomy ring fingers as [RingFingers, RingFingers], not [RingFingerPrimary, RingFingerSecondary].
            var equipperBodyPlanSlotKeys = Vals.BodyPlan.EquipmentSlotMapping.GetValueOrDefault(equipperAnatomy.BodyPlan, null)
                ?.SelectMany(kvp => kvp.Value.Select(v => kvp.Key))
                .Intersect(gearEquippable.EquipmentSlots)
                .ToList();

            if (equipperBodyPlanSlotKeys == null || equipperBodyPlanSlotKeys.Count() != gearEquippable.EquipmentSlots.Count)
            {
                output.Data = $"The {gearEntityName.GeneralName} doesn't fit {equipperEntityName.ProperName}'s anatomy.";

                return output;
            }

            var equipperAllSlots = Vals.BodyPlan.EquipmentSlotMapping.GetValueOrDefault(equipperAnatomy.BodyPlan, null)
                ?.SelectMany(kvp => kvp.Value.Select(v => new { kvp.Key, Value = v }))
                .ToList();
            var equipperBlockedSlots = equipperAnatomyModifications.Select(m => m.BodyPlanEquipmentSlotDisabled).ToList();
            var equipperActiveSlots = 

            //var wielderBodyPlanHandSlots = Vals.BodyPlan.EquipmentSlotMapping.GetValueOrDefault(wielderAnatomy.BodyPlan, null)
            //    ?.GetValueOrDefault(Vals.EquipmentSlots.WieldingHands, null)
            //    ?? new string[] { };
            //var wielderBlockedSlots = wielderAnatomyModifications.Select(m => m.BodyPlanEquipmentSlotDisabled).ToList();
            //var wielderActiveHandSlots = wielderBodyPlanHandSlots.Except(wielderBlockedSlots).ToList();
            //var wielderFreeHandSlots = wielderActiveHandSlots
            //    .Except(wielderAnatomy.SlotsEquipped.Keys.Intersect(wielderBodyPlanHandSlots)).ToList();



        }

        // MWCTODO: maybe this could be one method for all equips . . . finish wielding first, then think about this.
        // MWCTODO: i'm thinking we want to be able to chain actions from a single UI input, so maybe autoUnwieldExisting shouldn't exist, there should just be an unequip followed by a chained equip.
        /// <summary>
        /// Equipping a shield is really wielding one, though we may present it as equipping to the user. Yes, a tangle.
        /// </summary>
        internal static Output WieldWeapon(EcsRegistrar rgs, long wielderId, long weaponId, bool autoUnwieldExisting)
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

            var wielderBodyPlanHandSlots = Vals.BodyPlan.EquipmentSlotMapping.GetValueOrDefault(wielderAnatomy.BodyPlan, null)
                ?.GetValueOrDefault(Vals.EquipmentSlots.WieldingHands, null) 
                ?? new string[] { };

            var wielderBlockedSlots = wielderAnatomyModifications.Select(m => m.BodyPlanEquipmentSlotDisabled).ToList();
            var wielderActiveHandSlots = wielderBodyPlanHandSlots.Except(wielderBlockedSlots).ToList();

            if ((int)handsRequired > wielderActiveHandSlots.Count())
            {
                var start = $"The {weaponEntityName.GeneralName} requires {(int)handsRequired} to wield, and {wielderEntityName.Pronoun} ";
                var finish = wielderActiveHandSlots.Count > 0 ? $"has only {wielderActiveHandSlots.Count}." : "doesn't have any!";
                output.Data = start + finish;

                return output;
            }

            var wielderFreeHandSlots = wielderActiveHandSlots
                .Except(wielderAnatomy.SlotsEquipped.Keys.Intersect(wielderBodyPlanHandSlots)).ToList();

            if (wielderFreeHandSlots.Count < (int)handsRequired && autoUnwieldExisting)
            {
                //MWCTODO these items should go into inventory, or perhaps onto the ground. they're just vanishing this way!
                //this is a very lazy way to handle, but autoUnwieldExisting is probably temporary anyway.
                foreach (string activeHandSlot in wielderActiveHandSlots)
                {
                    wielderAnatomy.SlotsEquipped.Remove(activeHandSlot);
                }

                wielderFreeHandSlots = wielderActiveHandSlots.Except(wielderAnatomy.SlotsEquipped.Keys.Intersect(wielderBodyPlanHandSlots)).ToList();
            }

            //for now, we will assume the wielder always wants to equip in the primary hand slot--but this is only a temporary simplification.
            if (wielderFreeHandSlots.Count >= (int)handsRequired)
            {
                if (handsRequired == WeaponHandsRequired.Two)
                {
                    wielderAnatomy.SlotsEquipped[wielderFreeHandSlots[0]] = weaponId;
                    wielderAnatomy.SlotsEquipped[wielderFreeHandSlots[1]] = weaponId;
                    output.Data = $"{wielderEntityName.ProperName} grips the {weaponEntityName.GeneralName} firmly in two hands.";
                }
                else if (handsRequired == WeaponHandsRequired.One)
                {
                    //had been trying to automatically do main/off hand swapping here. this was a mistake. 
                    // if you make swapping hands a free action this means the whole main/off thing becomes much simpler.
                    wielderAnatomy.SlotsEquipped[wielderFreeHandSlots[0]] = weaponId;
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
            //MWCTODO: deal with pobjs of "none". will need to extend WeaponHandsRequired

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