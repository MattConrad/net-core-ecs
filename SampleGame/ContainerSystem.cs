using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    /// <summary>
    /// Any entity can be a container, and can be multiple containers. Usually a container will list entities that are "in" the containing entity,
    /// but it could have things that are "on" or "under", or all 3 at once.
    /// Let's try having "equipped" be just another container. At least as a test drive. (UPDATE: equipment also needs to fit a particlar slot, so maybe "container" is too simple. or maybe equipment is a container that works with another component . . . hmmm)
    /// A Battlefield is an entity that is mainly containers.
    /// However, a "region" won't be just a container, despite being somewhat containerish. (UPDATE: why not? maybe "just a container" is too simple. several containers?)
    /// </summary>
    internal static class CpContainer
    {
        internal static class Keys
        {
            /// <summary>
            /// HashSet[long]. Entity ids contained in this container.
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

        internal static class Vals
        {
            internal static class ContainerDescription
            {
                public const string Inventory = nameof(Inventory);
            }
        }

        /// <summary>
        /// Create an un-ided component.
        /// </summary>
        internal static EcsComponent Create(IEnumerable<long> containedEntityIds, string containerDescription = "", bool itemsAreVisible = false, string containment = "in")
        {
            var cp = new EcsComponent { Type = nameof(CpContainer), Data = new DataDict() };

            cp.Data[Keys.ContainedEntityIds] = containedEntityIds == null
                ? new HashSet<long>()
                : new HashSet<long>(containedEntityIds);

            cp.Data[Keys.ContainerDescription] = containerDescription ?? "";
            cp.Data[Keys.ItemsAreVisible] = itemsAreVisible;
            cp.Data[Keys.Containment] = containment ?? "";

            return cp;
        }
    }

    internal static class AlterContainerContentsResultsMessage
    {
        internal static class Keys
        {
            /// <summary>
            /// Bool.
            /// </summary>
            public const string Succeeded = nameof(Succeeded);

            /// <summary>
            /// List[string]. Unless this needs to be a DataDict.
            /// </summary>
            public const string Output = nameof(Output);
        }
    }

    internal static class ContainerSystem
    {
        public enum Action
        {
            AddEntity,
            RemoveEntity
        }

        //eventually entities bearing containers may have weight, size, inventory count, or other limits. for now, simple.
        internal static DataDict Add(EcsRegistrar rgs, long ownerId, long toAddId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toAddId, containerId);
        }

        internal static DataDict Remove(EcsRegistrar rgs, long ownerId, long toRemoveId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toRemoveId, containerId);
        }

        internal static HashSet<long> GetEntityIdsFromFirstContainerByDesc(EcsRegistrar rgs, long ownerId, string containerDescription)
        {
            var container = rgs.GetComponentsOfType(ownerId, nameof(CpContainer))
                .FirstOrDefault(c => c.Data.GetString(CpContainer.Keys.ContainerDescription) == containerDescription);

            return container.Data.GetHashSetLong(CpContainer.Keys.ContainedEntityIds);
        }

        internal static DataDict AddOrRemove(Action action, EcsRegistrar rgs, long ownerId, long toAddOrRemoveId, long? containerId = null)
        {
            //we might have many containers for an entity later, in which case containerId or other filtering may matter.
            var container = rgs.GetComponentsOfType(ownerId, nameof(CpContainer)).First();
            var ids = container.Data.GetHashSetLong(CpContainer.Keys.ContainedEntityIds);

            bool succeeded = false;
            if (action == Action.AddEntity && !ids.Contains(toAddOrRemoveId))
            {
                ids.Add(toAddOrRemoveId);
                succeeded = true;
            }

            if (action == Action.RemoveEntity && ids.Contains(toAddOrRemoveId))
            {
                ids.Remove(toAddOrRemoveId);
                succeeded = true;
            }

            var results = new DataDict();
            results[AlterContainerContentsResultsMessage.Keys.Succeeded] = succeeded;

            var ownerName = rgs.GetComponentsOfType(ownerId, nameof(CpEntityName)).FirstOrDefault();
            var addedName = rgs.GetComponentsOfType(toAddOrRemoveId, nameof(CpEntityName)).FirstOrDefault();

            //probably, eventually, we want the container 
            if (ownerName != null && addedName != null)
            {
                string baseMessage = action == Action.AddEntity ? "Added {0} to {1}." : "Removed {0} from {1}.";

                results[nameof(AlterContainerContentsResultsMessage.Keys.Output)] =
                    string.Format(baseMessage,
                        addedName.Data.GetString(CpEntityName.Keys.GeneralName),
                        ownerName.Data.GetString(CpEntityName.Keys.GeneralName));
            }

            return results;
        }
    }
}
