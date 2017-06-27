using System.Collections.Generic;

namespace SampleGame.Parts
{
    /// <summary>
    /// Any entity can be a container, and can be multiple containers. Usually a container will list entities that are "in" the containing entity,
    /// but it could have things that are "on" or "under", or all 3 at once.
    /// However, a "region" won't be just a container, despite being somewhat containerish. (UPDATE: why not? maybe "just a container" is too simple. several containers?)
    /// </summary>
    public class Container : EntropyEcsCore.EcsEntityPart
    {
        /// <summary>
        /// Entity ids in this container.
        /// </summary>
        public HashSet<long> EntityIds { get; set; } = new HashSet<long>();
        
        /// <summary>
        /// This is the tag used as a filter when trying to find a specific container.
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// If the entity with this part isn't primarily a container, this is the description for the container part of the entity. e.g., "pack" for a person, or "trunk" for a car.
        /// </summary>
        public string Description { get; set; }

        public bool ItemsAreVisible { get; set; } = false;
        
        /// <summary>
        /// String. Usually "in", but could be "on", or "equipped".
        /// </summary>
        public string Preposition { get; set; }
 
    }
}
