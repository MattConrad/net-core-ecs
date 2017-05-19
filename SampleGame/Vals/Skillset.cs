using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public static class EntityAttribute
    {
        public const string Physical = nameof(Physical);
        public const string Magical = nameof(Magical);
    }

    public static class EntitySkillPhysical
    {
        public const string Melee = nameof(Melee);
        public const string Dodge = nameof(Dodge);
    }
}
