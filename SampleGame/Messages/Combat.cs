using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Messages
{
    /// <summary>
    /// A combat action might have multiple targets, so there might be multiple combat messages from the same action.
    /// Not sure yet if the same action might have multiple messages for the same target, or just try to get all that with tags.
    /// </summary>
    internal class Combat
    {
        internal long Tick { get; set; }
        internal long ActorId { get; set; }
        internal long ActeeId { get; set; }
        internal string ActorCategory { get; set; }
        internal string ActeeCategory { get; set; }
        internal List<string> ActorTags { get; set; } = new List<string>();
        internal List<string> ActeeTags { get; set; } = new List<string>();
    }

    // These tags are kind of redundant with other declarations.
    //  I do not think I want this redundancy forever. Somehow those declarations should be shared.
    //  Maybe we need a Constants folder.

    public static class Categories
    {
        public const string MeleeAttack = nameof(MeleeAttack);
        public const string Dodge = nameof(Dodge);
    }

    public static class Tags
    {


    }

}
