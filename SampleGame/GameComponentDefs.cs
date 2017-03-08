using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleGame.ComponentDefs
{
    public static class CpEntityName
    {
        public static class Keys
        {
            public const string ProperName = nameof(ProperName);
            public const string VagueName = nameof(VagueName);
            public const string ShortDescription = nameof(ShortDescription);
            public const string LongDescription = nameof(LongDescription);
        }
    }

    /// <summary>
    /// This component is always present on entities that are in-game objects.
    /// It provides default behaviors for physical objects (and perhaps sounds, golden glows, etc).
    /// Most objects will include other components for more specific behavior.
    /// </summary>
    public static class CpWorldObject
    {
        public static class Keys
        {
            /// <summary>
            /// Bool. Some world objects will be incorporeal, and have special or absent behavior compared to normal objects.
            /// </summary>
            public const string Corporeal = nameof(Corporeal);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string Condition = nameof(Condition);

            /// <summary>
            /// Decimal, weight in pounds.
            /// </summary>
            public const string Weight = nameof(Weight);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string Size = nameof(Size);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string SizeModifier = nameof(SizeModifier);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string CarryableModifier = nameof(CarryableModifier);

            /// <summary>
            /// String: enum
            /// </summary>
            public const string WieldableModifier = nameof(WieldableModifier);
        }

        public static class Vals
        {
            public static class Condition
            {
                public const string Destroyed = nameof(Destroyed);
                public const string NearlyDestroyed = nameof(NearlyDestroyed);
                public const string Damaged = nameof(Damaged);
                public const string SlightlyDamaged = nameof(SlightlyDamaged);
                public const string Undamaged = nameof(Undamaged);
            }

            public static class Size
            {
                //hmmm, is this a good way to handle?
                public const string Miniscule = nameof(Miniscule);
                public const string Tiny = nameof(Tiny);
                public const string Small = nameof(Small);
                public const string Medium = nameof(Medium);
                public const string Big = nameof(Big);
                public const string VeryBig = nameof(VeryBig);
                public const string Enormous = nameof(Enormous);
            }

            public static class DamageType
            {
                public const string Mechanical = nameof(Mechanical);
                public const string Heat = nameof(Heat);
                public const string Electric = nameof(Electric);
                public const string Poison = nameof(Poison);
            }
        }
    }

    /// <summary>
    /// Anything can be a container, and can be multiple containers. Usually a container will list entities that are "in" the containing entity,
    /// but it could have things that are "on" or "under", or all 3 at once.
    /// Let's try having "equipped" be just another container. At least as a test drive.
    /// However, a "region" won't be just a container, despite being somewhat containerish.
    /// </summary>
    public static class CpContainer
    {
        public static class Keys
        {
            /// <summary>
            /// List[long]. Entity ids contained in this container.  MWCTODO: ugh, should this be HashSet instead?
            /// </summary>
            public const string ContainedEntityIds = nameof(ContainedEntityIds);

            /// <summary>
            /// String. If the entity with this component isn't primarily a container, this is the description for the container part of the entity. e.g., "pack" for a person, or "trunk" for a car.
            /// </summary>
            public const string ContainerDescription = nameof(ContainerDescription);

            /// <summary>
            /// Bool.
            /// </summary>
            public const string ItemsAreVisible = nameof(ItemsAreVisible);

            /// <summary>
            /// String. Usually "in", but could be "on", or "equipped".
            /// </summary>
            public const string Containment = nameof(Containment);
        }


    }



}
