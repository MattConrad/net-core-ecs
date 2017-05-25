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