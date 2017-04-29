using System.Collections.Generic;

namespace SampleGame.Parts
{
    /// <summary>
    /// Any entity can be a container, and can be multiple containers. Usually a container will list entities that are "in" the containing entity,
    /// but it could have things that are "on" or "under", or all 3 at once.
    /// Let's try having "equipped" be just another container. At least as a test drive. (UPDATE: equipment also needs to fit a particlar slot, so maybe "container" is too simple. or maybe equipment is a container that works with another part . . . hmmm)
    /// A Battlefield is an entity that is mainly containers.
    /// However, a "region" won't be just a container, despite being somewhat containerish. (UPDATE: why not? maybe "just a container" is too simple. several containers?)
    /// </summary>
    internal class Container : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// Entity ids in this container.
        /// </summary>
        internal HashSet<long> ContainedEntityIds { get; set; } = new HashSet<long>();
        //MWCTODO: maybe this wants to be an enum, or maybe we want to set up static strings like before. . . . static strings allow some flexibility.
        /// <summary>
        /// If the entity with this part isn't primarily a container, this is the description for the container part of the entity. e.g., "pack" for a person, or "trunk" for a car.
        /// </summary>
        internal string ContainerDescription { get; set; }
        internal bool ItemsAreVisible { get; set; }
        /// <summary>
        /// String. Usually "in", but could be "on", or "equipped".
        /// </summary>
        internal string Containment { get; set; }
    }
}
