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

    /// <summary>
    /// These are based on a humanoid body plan. We may want a xlation table for non humanoids, 
    /// but I don't have a clear idea of how that would work, especially for hexapodes/octopodes/etc.
    /// Notice that wielding hands and armor hand are different slots. Wielding is just a
    /// a specialized form of equipping. This is kind of a hack, not sure it will hold up, but trying it.
    /// </summary>
    public static class EquipmentSlot
    {
        public const string Head = nameof(Head);
        public const string Neck = nameof(Neck);
        public const string Body = nameof(Body);
        public const string WieldingHandPrimary = nameof(WieldingHandPrimary);
        public const string WieldingHandSecondary = nameof(WieldingHandSecondary);
        public const string Hands = nameof(Hands);
        public const string FingerPrimary = nameof(FingerPrimary);
        public const string FingerSecondary = nameof(FingerSecondary);
        public const string Foot = nameof(Foot);
        //not sure what, if anything, this does yet.
        public const string Special = nameof(Special);
    }

}
