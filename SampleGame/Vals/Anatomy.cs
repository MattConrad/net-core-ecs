using System;
using System.Collections.Generic;

namespace SampleGame.Vals
{
    public static class BodyPlan
    {
        public const string Human = nameof(Human);
        //public const string Quadruped = nameof(Quadruped);
        //public const string Avian = nameof(Avian);
        //public const string Insect = nameof(Insect);

        public static readonly Dictionary<string, BodySlots> BodyPlanToSlots = new Dictionary<string, BodySlots>
            {
                [Human] = BodySlots.WieldHandRight |
                    BodySlots.WieldHandLeft |
                    BodySlots.WieldTwoHanded |
                    BodySlots.HumanHeadHat |
                    BodySlots.HumanHeadHelmet |
                    BodySlots.HumanNeck |
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
                    BodySlots.HumanEyeLeft |
                    BodySlots.HumanEyeRight |
                    BodySlots.HumanEarLeft |
                    BodySlots.HumanEarRight |
                    BodySlots.HumanNose |
                    BodySlots.HumanMouth
            };

        /// <summary>
        /// This is only used to initialize the actual anatomy component for the first time. It should not be referenced
        /// for any entity once anatomy creation is complete.
        /// </summary>
        public static readonly Dictionary<string, string[]> OldBodyPlanSlots = new Dictionary<string, string[]>
        {
            [Human] = new string[]
            {
                BodyEquipmentSlots.WieldObjectAppendage,
                BodyEquipmentSlots.WieldObjectAppendage,
                BodyEquipmentSlots.VertebrateEye,
                BodyEquipmentSlots.VertebrateEye,
                BodyEquipmentSlots.VertebrateEar,
                BodyEquipmentSlots.VertebrateEar,
                BodyEquipmentSlots.VertebrateNose,
                BodyEquipmentSlots.VertebrateMouth,
                BodyEquipmentSlots.HumanoidHead,
                BodyEquipmentSlots.HumanoidNeck,
                BodyEquipmentSlots.HumanoidBody,
                BodyEquipmentSlots.HumanoidGauntletHandPair,
                BodyEquipmentSlots.HumanoidBootFootPair,
                BodyEquipmentSlots.HumanoidRingFinger,
                BodyEquipmentSlots.HumanoidRingFinger,
                BodyEquipmentSlots.HumanoidChest,
                BodyEquipmentSlots.HumanoidBack,
                BodyEquipmentSlots.HumanoidAbdomen,
                BodyEquipmentSlots.HumanoidGroin,
                BodyEquipmentSlots.HumanoidArm,
                BodyEquipmentSlots.HumanoidArm,
                BodyEquipmentSlots.HumanoidLeg,
                BodyEquipmentSlots.HumanoidLeg,
                BodyEquipmentSlots.HumanoidFoot,
                BodyEquipmentSlots.HumanoidFoot
            }
        };
    }

    public static class BodyEquipmentSlots
    {
        public const string Special = nameof(Special);

        public const string WieldObjectAppendage = nameof(WieldObjectAppendage);

        public const string VertebrateEye = nameof(VertebrateEye);
        public const string VertebrateEar = nameof(VertebrateEar);
        public const string VertebrateNose = nameof(VertebrateNose);
        public const string VertebrateMouth = nameof(VertebrateMouth);

        public const string HumanoidHead = nameof(HumanoidHead);
        public const string HumanoidNeck = nameof(HumanoidNeck);
        public const string HumanoidBody = nameof(HumanoidBody);
        public const string HumanoidGauntletHandPair = nameof(HumanoidGauntletHandPair);
        public const string HumanoidBootFootPair = nameof(HumanoidBootFootPair);
        public const string HumanoidRingFinger = nameof(HumanoidRingFinger);
        public const string HumanoidChest = nameof(HumanoidChest);
        public const string HumanoidBack = nameof(HumanoidBack);
        public const string HumanoidAbdomen = nameof(HumanoidAbdomen);
        public const string HumanoidGroin = nameof(HumanoidGroin);
        public const string HumanoidArm = nameof(HumanoidArm);
        public const string HumanoidLeg = nameof(HumanoidLeg);
        public const string HumanoidFoot = nameof(HumanoidFoot);

        public const string QuadrupedHead = nameof(QuadrupedHead);
        public const string QuadrupedNeck = nameof(QuadrupedNeck);
        public const string QuadrupedBody = nameof(QuadrupedBody);
        public const string QuadrupedFourHoofSet = nameof(QuadrupedFourHoofSet);
        public const string QuadrupedChest = nameof(QuadrupedChest);
        public const string QuadrupedBack = nameof(QuadrupedBack);
        public const string QuadrupedAbdomen = nameof(QuadrupedAbdomen);
        public const string QuadrupedGroin = nameof(QuadrupedGroin);
        public const string QuadrupedLeg = nameof(QuadrupedLeg);
        public const string QuadrupedFoot = nameof(QuadrupedFoot);
        public const string QuadrupedTail = nameof(QuadrupedTail);

        public const string AvianBeak = nameof(AvianBeak);
        public const string AvianWing = nameof(AvianWing);

        public const string InsectThorax = nameof(InsectThorax);
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
        HumanTorso = 0b000000000000000000000000000000000000000000000000000000001000000,
        HumanWaist = 0b000000000000000000000000000000000000000000000000000000010000000,
        HumanGroin = 0b000000000000000000000000000000000000000000000000000000100000000,
        HumanGlovesPair = 0b000000000000000000000000000000000000000000000000000001000000000,
        HumanBootsPair = 0b000000000000000000000000000000000000000000000000000010000000000,
        HumanArmLeft = 0b000000000000000000000000000000000000000000000000000100000000000,
        HumanArmRight = 0b000000000000000000000000000000000000000000000000001000000000000,
        HumanLegLeft = 0b000000000000000000000000000000000000000000000000010000000000000,
        HumanLegRight = 0b000000000000000000000000000000000000000000000000100000000000000,
        HumanWristLeft = 0b000000000000000000000000000000000000000000000001000000000000000,
        HumanWristRight = 0b000000000000000000000000000000000000000000000010000000000000000,
        HumanRingFingerLeft = 0b000000000000000000000000000000000000000000000100000000000000000,
        HumanRingFingerRight = 0b000000000000000000000000000000000000000000001000000000000000000,
        HumanEyeLeft = 0b000000000000000000000000000000000000000000010000000000000000000,
        HumanEyeRight = 0b000000000000000000000000000000000000000000100000000000000000000,
        HumanEarLeft = 0b000000000000000000000000000000000000000001000000000000000000000,
        HumanEarRight = 0b000000000000000000000000000000000000000010000000000000000000000,
        HumanNose = 0b000000000000000000000000000000000000000100000000000000000000000,
        HumanMouth = 0b000000000000000000000000000000000000001000000000000000000000000,
        //and we'll pause here until we actually need some of these others.
        Flags26 = 0b000000000000000000000000000000000000010000000000000000000000000,
        Flags27 = 0b000000000000000000000000000000000000100000000000000000000000000,
        Flags28 = 0b000000000000000000000000000000000001000000000000000000000000000,
        Flags29 = 0b000000000000000000000000000000000010000000000000000000000000000,
        Flags30 = 0b000000000000000000000000000000000100000000000000000000000000000,
        Flags31 = 0b000000000000000000000000000000001000000000000000000000000000000,
        Flags32 = 0b000000000000000000000000000000010000000000000000000000000000000,
        Flags33 = 0b000000000000000000000000000000100000000000000000000000000000000,
        Flags34 = 0b000000000000000000000000000001000000000000000000000000000000000,
        Flags35 = 0b000000000000000000000000000010000000000000000000000000000000000,
        Flags36 = 0b000000000000000000000000000100000000000000000000000000000000000,
        Flags37 = 0b000000000000000000000000001000000000000000000000000000000000000,
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

    /// <summary>
    /// These are both keys for the natural weapons dictionary, and also the names of the natural weapons blueprint.
    /// </summary>
    public static class NaturalWeaponNames
    {
        public const string HumanPunch = "obj.weapon.natural.humanoid.standard.punch";
    }



}
