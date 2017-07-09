using System;
using System.Collections.Generic;

namespace SampleGame.Vals
{
    public static class NaturalWeaponSet
    {
        public const string Human = nameof(Human);
        public const string Wolf = nameof(Wolf);
    }

    /// <summary>
    /// These are both keys for the natural weapons dictionary, and also the names of the natural weapons blueprint.
    /// </summary>
    public static class NaturalWeaponNames
    {
        public const string HumanPunch = "obj.weapon.natural.human.punch";
        public const string WolfBite = "obj.weapon.natural.wolf.bite";
    }

    public static class BodyPlan
    {
        public const string Human = nameof(Human);
        public const string Quadruped = nameof(Quadruped);
        //public const string Avian = nameof(Avian);
        //public const string Insect = nameof(Insect);

        /// <summary>
        /// It is important that any body plan that has WieldTwoHanded also have WieldHandRight and WieldHandLeft.
        /// If not, we'll get crashes. The reverse is not true--you can safely have only L/R wielding hands without the 2-h slot.
        /// </summary>
        public static readonly Dictionary<string, BodySlots> BodyPlanToSlots = new Dictionary<string, BodySlots>
            {
                [Human] = BodySlots.WieldHandRight |
                    BodySlots.WieldHandLeft |
                    BodySlots.WieldTwoHanded |
                    BodySlots.HumanHeadHat |
                    BodySlots.HumanHeadHelmet |
                    BodySlots.HumanNeck |
                    BodySlots.HumanBodyArmor |
                    BodySlots.HumanTorso |
                    BodySlots.HumanWaist |
                    BodySlots.HumanGroin |
                    BodySlots.HumanGlovesPair |
                    BodySlots.HumanBootsPair |
                    BodySlots.HumanArmLeft |
                    BodySlots.HumanArmRight |
                    BodySlots.HumanLegLeft |
                    BodySlots.HumanLegRight |
                    BodySlots.HumanWristLeft |
                    BodySlots.HumanWristRight |
                    BodySlots.HumanRingFingerLeft |
                    BodySlots.HumanRingFingerRight |
                    BodySlots.VertebrateEyeLeft |
                    BodySlots.VertebrateEyeRight |
                    BodySlots.VertebrateEarLeft |
                    BodySlots.VertebrateEarRight |
                    BodySlots.VertebrateNose |
                    BodySlots.VertebrateMouth,
                [Quadruped] = BodySlots.QuadrupedHead |
                    BodySlots.QuadrupedNeck |
                    BodySlots.QuadrupedBarding |
                    BodySlots.QuadrupedTorso |
                    BodySlots.QuadrupedBack |
                    BodySlots.QuadrupedRightForeLeg |
                    BodySlots.QuadrupedLeftForeLeg |
                    BodySlots.QuadrupedRightHindLeg |
                    BodySlots.QuadrupedLeftHindLeg |
                    BodySlots.QuadrupedTail |
                    BodySlots.VertebrateEyeLeft |
                    BodySlots.VertebrateEyeRight |
                    BodySlots.VertebrateEarLeft |
                    BodySlots.VertebrateEarRight |
                    BodySlots.VertebrateNose |
                    BodySlots.VertebrateMouth
        };

        public static readonly Dictionary<BodySlots, BodySlots[]> BodySlotsEncompassedByOtherSlot = new Dictionary<BodySlots, BodySlots[]>
            {
                [BodySlots.WieldTwoHanded] = new BodySlots[] { BodySlots.WieldHandLeft, BodySlots.WieldHandRight },
                [BodySlots.HumanBodyArmor] = new BodySlots[] { BodySlots.HumanArmLeft, BodySlots.HumanArmRight, BodySlots.HumanLegLeft, BodySlots.HumanLegRight, BodySlots.HumanGroin }
            };
    }

    [Flags]
    public enum BodySlots : long
    {
        NoSlot = 0b000000000000000000000000000000000000000000000000000000000000000,
        WieldHandRight = 0b000000000000000000000000000000000000000000000000000000000000001,
        WieldHandLeft = 0b000000000000000000000000000000000000000000000000000000000000010,
        WieldTwoHanded = 0b000000000000000000000000000000000000000000000000000000000000100,
        HumanHeadHat = 0b000000000000000000000000000000000000000000000000000000000001000,
        HumanHeadHelmet = 0b000000000000000000000000000000000000000000000000000000000010000,
        HumanNeck = 0b000000000000000000000000000000000000000000000000000000000100000,
        HumanBodyArmor = 0b000000000000000000000000000000000000000000000000000000001000000,
        HumanTorso = 0b000000000000000000000000000000000000000000000000000000010000000,
        HumanBack = 0b000000000000000000000000000000000000000000000000000000100000000,
        HumanWaist = 0b000000000000000000000000000000000000000000000000000001000000000,
        HumanGroin = 0b000000000000000000000000000000000000000000000000000010000000000,
        HumanGlovesPair = 0b000000000000000000000000000000000000000000000000000100000000000,
        HumanBootsPair = 0b000000000000000000000000000000000000000000000000001000000000000,
        HumanArmLeft = 0b000000000000000000000000000000000000000000000000010000000000000,
        HumanArmRight = 0b000000000000000000000000000000000000000000000000100000000000000,
        HumanLegLeft = 0b000000000000000000000000000000000000000000000001000000000000000,
        HumanLegRight = 0b000000000000000000000000000000000000000000000010000000000000000,
        HumanWristLeft = 0b000000000000000000000000000000000000000000000100000000000000000,
        HumanWristRight = 0b000000000000000000000000000000000000000000001000000000000000000,
        HumanRingFingerLeft = 0b000000000000000000000000000000000000000000010000000000000000000,
        HumanRingFingerRight = 0b000000000000000000000000000000000000000000100000000000000000000,
        VertebrateEyeLeft = 0b000000000000000000000000000000000000000001000000000000000000000,
        VertebrateEyeRight = 0b000000000000000000000000000000000000000010000000000000000000000,
        VertebrateEarLeft = 0b000000000000000000000000000000000000000100000000000000000000000,
        VertebrateEarRight = 0b000000000000000000000000000000000000001000000000000000000000000,
        VertebrateNose = 0b000000000000000000000000000000000000010000000000000000000000000,
        VertebrateMouth = 0b000000000000000000000000000000000000100000000000000000000000000,
        QuadrupedHead = 0b000000000000000000000000000000000001000000000000000000000000000,
        QuadrupedNeck = 0b000000000000000000000000000000000010000000000000000000000000000,
        QuadrupedBarding = 0b000000000000000000000000000000000100000000000000000000000000000,
        QuadrupedTorso = 0b000000000000000000000000000000001000000000000000000000000000000,
        QuadrupedBack = 0b000000000000000000000000000000010000000000000000000000000000000,
        QuadrupedRightForeLeg = 0b000000000000000000000000000000100000000000000000000000000000000,
        QuadrupedLeftForeLeg = 0b000000000000000000000000000001000000000000000000000000000000000,
        QuadrupedRightHindLeg = 0b000000000000000000000000000010000000000000000000000000000000000,
        QuadrupedLeftHindLeg = 0b000000000000000000000000000100000000000000000000000000000000000,
        QuadrupedTail = 0b000000000000000000000000001000000000000000000000000000000000000,
        Flags38 = 0b000000000000000000000000010000000000000000000000000000000000000,
        Flags39 = 0b000000000000000000000000100000000000000000000000000000000000000,
        Flags40 = 0b000000000000000000000001000000000000000000000000000000000000000,
        Flags41 = 0b000000000000000000000010000000000000000000000000000000000000000,
        Flags42 = 0b000000000000000000000100000000000000000000000000000000000000000,
        Flags43 = 0b000000000000000000001000000000000000000000000000000000000000000,
        Flags44 = 0b000000000000000000010000000000000000000000000000000000000000000,
        Flags45 = 0b000000000000000000100000000000000000000000000000000000000000000,
        Flags46 = 0b000000000000000001000000000000000000000000000000000000000000000,
        Flags47 = 0b000000000000000010000000000000000000000000000000000000000000000,
        Flags48 = 0b000000000000000100000000000000000000000000000000000000000000000,
        Flags49 = 0b000000000000001000000000000000000000000000000000000000000000000,
        Flags50 = 0b000000000000010000000000000000000000000000000000000000000000000,
        Flags51 = 0b000000000000100000000000000000000000000000000000000000000000000,
        Flags52 = 0b000000000001000000000000000000000000000000000000000000000000000,
        Flags53 = 0b000000000010000000000000000000000000000000000000000000000000000,
        Flags54 = 0b000000000100000000000000000000000000000000000000000000000000000,
        Flags55 = 0b000000001000000000000000000000000000000000000000000000000000000,
        Flags56 = 0b000000010000000000000000000000000000000000000000000000000000000,
        Flags57 = 0b000000100000000000000000000000000000000000000000000000000000000,
        Flags58 = 0b000001000000000000000000000000000000000000000000000000000000000,
        Flags59 = 0b000010000000000000000000000000000000000000000000000000000000000,
        Flags60 = 0b000100000000000000000000000000000000000000000000000000000000000,
        Flags61 = 0b001000000000000000000000000000000000000000000000000000000000000,
        Flags62 = 0b010000000000000000000000000000000000000000000000000000000000000,
        Special = 0b100000000000000000000000000000000000000000000000000000000000000
    }

}
