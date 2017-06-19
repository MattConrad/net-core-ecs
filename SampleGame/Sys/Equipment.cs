using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    //MWCTODO: should this be the Anatomy system? do we want sys names to closely map to part names? this relies on anatomy parts. 

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
        //internal static int _defaultWielderSizeInt = Vals.Size.SizeToInt[Vals.Size.Human];
        //internal static int _defaultWeaponWieldSizeInt = Vals.Size.SizeToInt[Vals.Size.Goliath];

        internal static Output Equip(EcsRegistrar rgs, long equipperId, long gearId, bool autoUnequipExisting)
        {
            var output = new Output { Category = "Text" };

            var equipperAnatomy = rgs.GetPartSingle<Parts.Anatomy>(equipperId);
            var equipperAnatomyModifications = rgs.GetParts<Parts.AnatomyModifier>(equipperId).ToList();
            var equipperPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(equipperId);
            var equipperEntityName = rgs.GetPartSingle<Parts.EntityName>(equipperId);
            var gearPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(gearId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            //someday we'll support gear that occupies multiple slots or varying slots.
            if (gearPhysicalObject.EquipmentSlotName == Vals.BodyEquipmentSlots.Special) throw new NotImplementedException();

            var equipperAllSlotNames = equipperAnatomy.SlotsEquipped.Select(k => k.Key).ToList();
            var equipperDisabledSlotNames = equipperAnatomyModifications.Select(m => m.EquipmentSlotDisabled).ToList();
            foreach (var disabled in equipperDisabledSlotNames)
            {
                equipperAllSlotNames.Remove(disabled);
            };

            if (!equipperAllSlotNames.Contains(gearPhysicalObject.EquipmentSlotName))
            {
                //MWCTODO: maybe this should be more descriptive. e.g. "doesn't have a working <head>!"
                output.Data = $"The {gearEntityName.GeneralName} doesn't fit {equipperEntityName.ProperName}'s anatomy.";

                return output;
            }

            if (!equipperAnatomy.SlotsEquipped.Any(s => s.Key == gearPhysicalObject.EquipmentSlotName && s.Value == 0))
            {
                output.Data = $"The {gearEntityName.GeneralName} already has gear equipped there. Remove something else first.";

                return output;
            }

            var firstOpenSlot = equipperAnatomy.SlotsEquipped.First(s => s.Key == gearPhysicalObject.EquipmentSlotName && s.Value == 0);
            equipperAnatomy.SlotsEquipped.Remove(firstOpenSlot);
            equipperAnatomy.SlotsEquipped.Add(new KeyValuePair<string, long>(gearPhysicalObject.EquipmentSlotName, gearId));

            output.Data = $"{equipperEntityName.ProperName} puts on the {gearEntityName.GeneralName}.";

            return output;
        }

        internal static Output NewEquip(EcsRegistrar rgs, long equipperId, long gearId)
        {
            var output = new Output { Category = "Text" };

            var equipperAnatomy = rgs.GetPartSingle<Parts.Anatomy>(equipperId);
            var equipperAnatomyModifications = rgs.GetParts<Parts.AnatomyModifier>(equipperId).ToList();
            var equipperPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(equipperId);
            var equipperEntityName = rgs.GetPartSingle<Parts.EntityName>(equipperId);
            var gearPhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(gearId);
            var gearEntityName = rgs.GetPartSingle<Parts.EntityName>(gearId);

            //someday we'll support gear that occupies multiple slots or varying slots.
            if (gearPhysicalObject.EquipmentSlots == Vals.BodySlots.NoSlot || gearPhysicalObject.EquipmentSlots == Vals.BodySlots.NoSlot) throw new NotImplementedException();

            //https://stackoverflow.com/questions/93744/most-common-c-sharp-bitwise-operations-on-enums

            var equipperSlots = equipperAnatomy.NewSlotsEquipped.ToDictionary(d => d.Key, d => d.Value);

            var equipperDisabledSlots = equipperAnatomyModifications.Select(m => m.NewEquipmentSlotsDisabled).ToList();

            https://stackoverflow.com/questions/4171140/iterate-over-values-in-flags-enum

            //foreach (var disabled in equipperDisabledSlotNames)
            //{
            //    equipperAllSlotNames.Remove(disabled);
            //};

            //if (!equipperAllSlotNames.Contains(gearPhysicalObject.EquipmentSlotName))
            //{
            //    //MWCTODO: maybe this should be more descriptive. e.g. "doesn't have a working <head>!"
            //    output.Data = $"The {gearEntityName.GeneralName} doesn't fit {equipperEntityName.ProperName}'s anatomy.";

            //    return output;
            //}

            //if (!equipperAnatomy.SlotsEquipped.Any(s => s.Key == gearPhysicalObject.EquipmentSlotName && s.Value == 0))
            //{
            //    output.Data = $"The {gearEntityName.GeneralName} already has gear equipped there. Remove something else first.";

            //    return output;
            //}

            //var firstOpenSlot = equipperAnatomy.SlotsEquipped.First(s => s.Key == gearPhysicalObject.EquipmentSlotName && s.Value == 0);
            //equipperAnatomy.SlotsEquipped.Remove(firstOpenSlot);
            //equipperAnatomy.SlotsEquipped.Add(new KeyValuePair<string, long>(gearPhysicalObject.EquipmentSlotName, gearId));

            //output.Data = $"{equipperEntityName.ProperName} puts on the {gearEntityName.GeneralName}.";

            return output;
        }

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

            //var handsRequired = GetWeaponHandsRequired(wielderPhysicalObject, weaponPhysicalObject);
            var handsRequired = WeaponHandsRequired.One;

            //MWCTODO: "to wield", what if it's a shield? (see remarks below about main/off hand auto-determination--we're not going to do that).
            if (handsRequired == WeaponHandsRequired.WeaponTooBig) output.Data = $"The {weaponEntityName.GeneralName} is too big for {wielderEntityName.Pronoun} to wield.";
            if (handsRequired == WeaponHandsRequired.WeaponTooSmall) output.Data = $"The {weaponEntityName.GeneralName} is too small for {wielderEntityName.Pronoun} to wield.";

            if (handsRequired == WeaponHandsRequired.WeaponTooBig || handsRequired == WeaponHandsRequired.WeaponTooSmall)
            {
                return output;
            }

            var wielderAllHandSlotNames = wielderAnatomy.SlotsEquipped.Select(k => k.Key).Where(s => s == Vals.BodyEquipmentSlots.WieldObjectAppendage).ToList();
            var wielderDisableddHandSlotNames = wielderAnatomyModifications.Select(m => m.EquipmentSlotDisabled).Where(s => s == Vals.BodyEquipmentSlots.WieldObjectAppendage).ToList();
            //body plans have lots of duplicate entries, so can't use set-based .Except().
            foreach (var disabled in wielderDisableddHandSlotNames)
            {
                wielderAllHandSlotNames.Remove(disabled);
            };

            if ((int)handsRequired > wielderAllHandSlotNames.Count())
            {
                var start = $"The {weaponEntityName.GeneralName} requires {(int)handsRequired} hands to wield, and {wielderEntityName.Pronoun} ";
                var finish = wielderAllHandSlotNames.Count > 0 ? $"has only {wielderAllHandSlotNames.Count}." : "doesn't have any!";
                output.Data = start + finish;

                return output;
            }

            var openWielderHandSlots = wielderAnatomy.SlotsEquipped.Where(s => s.Key == Vals.BodyEquipmentSlots.WieldObjectAppendage && s.Value == 0).ToList();
            ////for now, we will assume the wielder always wants to equip in the primary hand slot--but this is only a temporary simplification.
            if (openWielderHandSlots.Count >= (int)handsRequired)
            {
                if (handsRequired == WeaponHandsRequired.Two)
                {
                    wielderAnatomy.SlotsEquipped.Remove(openWielderHandSlots[0]);
                    wielderAnatomy.SlotsEquipped.Remove(openWielderHandSlots[1]);
                    wielderAnatomy.SlotsEquipped.Add(new KeyValuePair<string, long>(Vals.BodyEquipmentSlots.WieldObjectAppendage, weaponId));
                    wielderAnatomy.SlotsEquipped.Add(new KeyValuePair<string, long>(Vals.BodyEquipmentSlots.WieldObjectAppendage, weaponId));

                    output.Data = $"{wielderEntityName.ProperName} grips the {weaponEntityName.GeneralName} firmly in two hands.";
                }
                else if (handsRequired == WeaponHandsRequired.One)
                {
                    //had been trying to automatically do main/off hand swapping here. this was a mistake. 
                    // if you make swapping hands a free action this means the whole main/off thing becomes much simpler.
                    wielderAnatomy.SlotsEquipped.Remove(openWielderHandSlots[0]);
                    wielderAnatomy.SlotsEquipped.Add(new KeyValuePair<string, long>(Vals.BodyEquipmentSlots.WieldObjectAppendage, weaponId));

                    output.Data = $"{wielderEntityName.ProperName} wields the {weaponEntityName.GeneralName}.";
                }
            }
            else
            {
                output.Data = $"{wielderEntityName.ProperName} already has {wielderEntityName.Pronoun}[poss-adj] hands full. Unequip something first.";
            }

            return output;
        }

        ///// <summary>
        ///// Hands required to wield a weapon/object.
        ///// Weapons matching or one step smaller than the wielder can be wielded one handed.
        ///// Weapons one step bigger than the wielder can be wielded 2 handed.
        ///// Weapons outside those ranges cannot be wielded.
        ///// </summary>
        //internal static WeaponHandsRequired GetWeaponHandsRequired(Parts.PhysicalObject wielderPhysicalObject, Parts.PhysicalObject weaponPhysicalObject)
        //{
        //    int wielderSizeInt = Vals.Size.SizeToInt.GetValueOrDefault(wielderPhysicalObject.Size ?? "", _defaultWielderSizeInt);
        //    int weaponWieldSizeInt = Vals.Size.SizeToInt.GetValueOrDefault(weaponPhysicalObject.WieldableSingleHandSize ?? "", _defaultWeaponWieldSizeInt);

        //    int delta = weaponWieldSizeInt - wielderSizeInt;

        //    if (delta < -1) return WeaponHandsRequired.WeaponTooSmall;

        //    if (delta == -1 || delta == 0) return WeaponHandsRequired.One;

        //    if (delta == 1) return WeaponHandsRequired.Two;

        //    return WeaponHandsRequired.WeaponTooBig;
        //}

        internal static Dictionary<string, long> InitializeNaturalWeapons(EcsRegistrar rgs)
        {
            var nwd = new Dictionary<string, long>();

            var names = new string[] { Vals.NaturalWeaponNames.HumanPunch };

            foreach(var name in names)
            {
                try
                {
                    var id = Blueprinter.GetEntityFromBlueprint(rgs, name);
                    nwd.Add(name, id);
                }
                catch { }
            }

            return nwd;
        }

    }
}