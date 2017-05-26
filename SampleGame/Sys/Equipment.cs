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

            //MWCTODO: "to wield", what if it's a shield?
            if (handsRequired == WeaponHandsRequired.WeaponTooBig) output.Data = $"The {weaponEntityName.GeneralName} is too big for {wielderEntityName.Pronoun} to wield.";
            if (handsRequired == WeaponHandsRequired.WeaponTooSmall) output.Data = $"The {weaponEntityName.GeneralName} is too small for {wielderEntityName.Pronoun} to wield.";

            if (handsRequired == WeaponHandsRequired.WeaponTooBig || handsRequired == WeaponHandsRequired.WeaponTooSmall)
            {
                return output;
            }

            //so human-centric! eventually this has to be fixed.
            var humanoidHandSlots = new List<string> { Vals.EquipmentSlotsHumanoid.WieldingHandPrimary, Vals.EquipmentSlotsHumanoid.WieldingHandOffhand };

            var wielderBlockedSlots = wielderAnatomyModifications.Select(m => m.SlotDisabled).ToList();
            var wielderActiveHandSlots = humanoidHandSlots.Except(wielderBlockedSlots).ToList();

            if ((int)handsRequired > wielderActiveHandSlots.Count())
            {
                var start = $"The {weaponEntityName.GeneralName} requires {(int)handsRequired} to wield, and {wielderEntityName.Pronoun} ";
                var finish = wielderActiveHandSlots.Count > 0 ? $"has only {wielderActiveHandSlots.Count}." : "doesn't have any!";
                output.Data = start + finish;

                return output;
            }

            var wielderFreeHandSlots = wielderActiveHandSlots.Except(wielderAnatomy.SlotsEquipped.Keys.Intersect(humanoidHandSlots)).ToList();

            if (wielderFreeHandSlots.Count < (int)handsRequired && autoUnwieldExisting)
            {
                //MWCTODO: stuff that is unequipped should go into the inventory, or onto the ground, but not just disappear.

                if (handsRequired == WeaponHandsRequired.Two && wielderAnatomy.SlotsEquipped.Keys.Contains((Vals.EquipmentSlotsHumanoid.WieldingHandOffhand)))
                {
                    wielderAnatomy.SlotsEquipped.Remove(Vals.EquipmentSlotsHumanoid.WieldingHandOffhand);
                }

                if (wielderAnatomy.SlotsEquipped.Keys.Contains((Vals.EquipmentSlotsHumanoid.WieldingHandOffhand)))
                {
                    wielderAnatomy.SlotsEquipped.Remove(Vals.EquipmentSlotsHumanoid.WieldingHandPrimary);
                }

                //there should be some output to the user here, maybe not super important given autoUnwieldExisting probably going away.

                wielderFreeHandSlots = wielderActiveHandSlots.Except(wielderAnatomy.SlotsEquipped.Keys.Intersect(humanoidHandSlots)).ToList();
            }

            //for now, we will assume the wielder always wants to equip in the primary hand slot--but this is only a temporary simplification.
            if (wielderFreeHandSlots.Count >= (int)handsRequired)
            {
                if (handsRequired == WeaponHandsRequired.Two)
                {
                    wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandPrimary] = weaponId;
                    wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandOffhand] = weaponId;
                    output.Data = $"{wielderEntityName.ProperName} grips the {weaponEntityName.GeneralName} firmly in both hands.";

                    return output;
                }
                else if (handsRequired == WeaponHandsRequired.One)
                {
                    //we already know they've got enough total slots, so if the primary slot is already full, they have to have a secondary slot open.
                    if (wielderAnatomy.SlotsEquipped.ContainsKey(Vals.EquipmentSlotsHumanoid.WieldingHandPrimary))
                    {
                        wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandOffhand] = 
                            wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandPrimary];
                        wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandPrimary] = weaponId;

                        output.Data = $"{wielderEntityName.ProperName} swaps gear to {wielderEntityName.Pronoun}[poss-adj] offhand and wields {weaponEntityName.GeneralName} in his primary hand.";

                        return output;
                    }
                    else
                    {
                        wielderAnatomy.SlotsEquipped[Vals.EquipmentSlotsHumanoid.WieldingHandPrimary] = weaponId;

                        output.Data = $"{wielderEntityName.ProperName} wields {weaponEntityName.GeneralName} in his primary hand.";

                        return output;
                    }

                }
            }

            output.Data = $"{wielderEntityName.ProperName} already has {wielderEntityName.Pronoun}[poss-adj] hands full. Unequip something first.";

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
            int wielderSizeInt = _defaultWielderSizeInt;
            Vals.Size.SizeToInt.TryGetValue(wielderPhysicalObject.Size, out wielderSizeInt);

            int weaponWieldSizeInt = _defaultWeaponWieldSizeInt;
            Vals.Size.SizeToInt.TryGetValue(weaponPhysicalObject.Size, out weaponWieldSizeInt);

            int delta = weaponWieldSizeInt - wielderSizeInt;

            if (delta < -1) return WeaponHandsRequired.WeaponTooSmall;

            if (delta == -1 || delta == 0) return WeaponHandsRequired.One;

            if (delta == 1) return WeaponHandsRequired.Two;

            return WeaponHandsRequired.WeaponTooBig;
        }

    }
}