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

        //MWCTODO: we need better names here. the key and value are not distinctly enough named. 
        // when I said BodyPlanEquipmentSlotDisabled in AnatomyModifier, it wasn't immediately clear what that meant.
        // really, this whole mapping is SUPER FLEXIBLE but it is also SUPER CONFUSING. i wrote it like 2 days ago
        // and I'm already all confused about how it works.
        // i'm now thinking this dictionary mapping has to go. something simpler where each body plan has a simple list of equipment slots.
        // it's OK if the same slot is duplicated, so humans would have [..., RingFinger, RingFinger, GauntletHands, ...]
        // i think we have to do this, or just be ??? every time we work with equipment and anatomy, which is no good.


        //if there are multiple mappings for a slot, the highest priority should always come first, and descending from there.
        public static readonly Dictionary<string, Dictionary<string, string[]>> EquipmentSlotMapping = new Dictionary<string, Dictionary<string, string[]>>
        {
            //never put an empty array in for an equipment slot--if the body type doesn't have any of that slot, omit the key entirely.
            [Humanoid] = new Dictionary<string, string[]>
            {
                [EquipmentSlots.Head] = new string[] { EquipmentSlotsHumanoid.Head },
                [EquipmentSlots.Neck] = new string[] { EquipmentSlotsHumanoid.Neck },
                [EquipmentSlots.Body] = new string[] { EquipmentSlotsHumanoid.Body },
                [EquipmentSlots.WieldingHands] = new string[] { EquipmentSlotsHumanoid.WieldingHandPrimary, EquipmentSlotsHumanoid.WieldingHandOffhand },
                [EquipmentSlots.GauntletHands] = new string[] { EquipmentSlotsHumanoid.GauntletHands },
                [EquipmentSlots.RingFingers] = new string[] { EquipmentSlotsHumanoid.RingFingerPrimary, EquipmentSlotsHumanoid.RingFingerSecondary },
                [EquipmentSlots.BootFeet] = new string[] { EquipmentSlotsHumanoid.BootFeet }
            },
            [Quadruped] = new Dictionary<string, string[]>
            {
                [EquipmentSlots.Head] = new string[] { EquipmentSlotsQuadruped.Head },
                [EquipmentSlots.Neck] = new string[] { EquipmentSlotsQuadruped.Neck }
            }
        };

        public static class EquipmentSlotsHumanoid
        {
            public const string Head = nameof(Head);
            public const string Neck = nameof(Neck);
            public const string Body = nameof(Body);
            public const string GauntletHands = nameof(GauntletHands);
            public const string BootFeet = nameof(BootFeet);
            public const string WieldingHandPrimary = nameof(WieldingHandPrimary);
            public const string WieldingHandOffhand = nameof(WieldingHandOffhand);
            public const string RingFingerPrimary = nameof(RingFingerPrimary);
            public const string RingFingerSecondary = nameof(RingFingerSecondary);
        }

        public static class EquipmentSlotsQuadruped
        {
            public const string Head = nameof(Head);
            public const string Neck = nameof(Neck);
        }
    }

    public static class BodyParts
    {
        public const string Head = nameof(Head);
        public const string Eye = nameof(Eye);
        public const string Ear = nameof(Ear);
        public const string Nose = nameof(Nose);
        public const string Mouth = nameof(Mouth);
        public const string Neck = nameof(Neck);
        public const string Chest = nameof(Chest);
        public const string Back = nameof(Back);
        public const string Abdomen = nameof(Abdomen);
        public const string Arm = nameof(Arm);
        public const string Hand = nameof(Hand);
        public const string Groin = nameof(Groin);
        public const string Leg = nameof(Leg);
        public const string Foot = nameof(Foot);

        public const string Tail = nameof(Tail);

        public const string Beak = nameof(Beak);
        public const string Wing = nameof(Wing);

        public const string Thorax = nameof(Thorax);
    }

}
