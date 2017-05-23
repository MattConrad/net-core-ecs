using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public static class BodyPlan
    {
        //we know which body plans include what parts, and we know what body plans are bilateral. i don't think we need to spell this out with vals.
        // we might want more detailed anatomy like wrists, knees, or hindleg/foreleg, though.

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
}
