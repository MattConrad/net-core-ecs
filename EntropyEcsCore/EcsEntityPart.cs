using System;
using System.Collections.Generic;
using System.Text;

namespace EntropyEcsCore
{
    public abstract class EcsEntityPart : IEquatable<EcsEntityPart>
    {
        public long Id { get; set; }

        public bool Equals(EcsEntityPart other)
        {
            if (other == null) return false;

            return (this.Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            EcsEntityPart part = obj as EcsEntityPart;

            if (part == null) return false;

            return Equals(part);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(EcsEntityPart part1, EcsEntityPart part2)
        {
            if (((object)part1) == null || ((object)part2) == null)
                return Object.Equals(part1, part2);

            return part1.Equals(part2);
        }

        public static bool operator !=(EcsEntityPart part1, EcsEntityPart part2)
        {
            if (((object)part1) == null || ((object)part2) == null)
                return !Object.Equals(part1, part2);

            return !(part1.Equals(part2));
        }
    }
}
