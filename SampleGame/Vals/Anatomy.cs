using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public static class BodyPlan
    {
        public const string Humanoid = nameof(Humanoid);
        public const string Quadruped = nameof(Quadruped);
        public const string Avian = nameof(Avian);
        public const string Insect = nameof(Insect);

        /// <summary>
        /// This is only used to initialize the actual anatomy component for the first time. It should not be referenced
        /// for any entity once anatomy creation is complete.
        /// </summary>
        public static readonly Dictionary<string, string[]> BodyPlanEquipmentSlots = new Dictionary<string, string[]>
        {
            [Humanoid] = new string[]
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
            },
            [Quadruped] = new string[]
            {
                BodyEquipmentSlots.VertebrateEye,
                BodyEquipmentSlots.VertebrateEye,
                BodyEquipmentSlots.VertebrateEar,
                BodyEquipmentSlots.VertebrateEar,
                BodyEquipmentSlots.VertebrateNose,
                BodyEquipmentSlots.VertebrateMouth,
                BodyEquipmentSlots.QuadrupedHead,
                BodyEquipmentSlots.QuadrupedNeck,
                BodyEquipmentSlots.QuadrupedBody,
                BodyEquipmentSlots.QuadrupedChest,
                BodyEquipmentSlots.QuadrupedBack,
                BodyEquipmentSlots.QuadrupedAbdomen,
                BodyEquipmentSlots.QuadrupedGroin,
                BodyEquipmentSlots.QuadrupedLeg,
                BodyEquipmentSlots.QuadrupedLeg,
                BodyEquipmentSlots.QuadrupedLeg,
                BodyEquipmentSlots.QuadrupedLeg,
                BodyEquipmentSlots.QuadrupedFoot,
                BodyEquipmentSlots.QuadrupedFoot,
                BodyEquipmentSlots.QuadrupedFoot,
                BodyEquipmentSlots.QuadrupedFoot,
                BodyEquipmentSlots.QuadrupedTail
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

}
