using System;
using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame
{
    //MWCTODO: how well will enums serialize/deserialize?

    //MWCTODO: this probably dies, but we're still figuring this section out.
    internal class AlterContainerContentsResultsMessage
    {
        public bool Succeeded { get; set; }
        public string Output { get; set; }
    }

    internal static class ContainerSystem
    {
        public enum Action
        {
            AddEntity,
            RemoveEntity
        }

        //eventually entities bearing containers may have weight, size, inventory count, or other limits. for now, simple.
        internal static AlterContainerContentsResultsMessage Add(EcsRegistrar rgs, long ownerId, long toAddId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toAddId, containerId);
        }

        internal static AlterContainerContentsResultsMessage Remove(EcsRegistrar rgs, long ownerId, long toRemoveId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toRemoveId, containerId);
        }

        internal static HashSet<long> GetEntityIdsFromFirstContainerByDesc(EcsRegistrar rgs, long ownerId, string containerDescription)
        {
            var container = rgs.GetPartsOfType<Parts.Container>(ownerId).FirstOrDefault(c => c.ContainerDescription == containerDescription);

            return container?.ContainedEntityIds ?? new HashSet<long>();
        }

        internal static AlterContainerContentsResultsMessage AddOrRemove(Action action, EcsRegistrar rgs, long ownerId, long toAddOrRemoveId, long? containerId = null)
        {
            //MWCTODO: we might have many containers for an entity later, so this is temporary only.
            var container = rgs.GetPartsOfType<Parts.Container>(ownerId).FirstOrDefault();

            if (container == null) return new AlterContainerContentsResultsMessage { Succeeded = false, Output = "Container not found." };

            var ids = container.ContainedEntityIds;

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

            //var ownerName = rgs.GetComponentsOfType(ownerId, nameof(CpEntityName)).FirstOrDefault();
            //var addedName = rgs.GetComponentsOfType(toAddOrRemoveId, nameof(CpEntityName)).FirstOrDefault();

            ////probably, eventually, we want the container 
            //if (ownerName != null && addedName != null)
            //{
            //    string baseMessage = action == Action.AddEntity ? "Added {0} to {1}." : "Removed {0} from {1}.";

            //    results[nameof(AlterContainerContentsResultsMessage.Keys.Output)] =
            //        string.Format(baseMessage,
            //            addedName.Data.GetString(CpEntityName.Keys.GeneralName),
            //            ownerName.Data.GetString(CpEntityName.Keys.GeneralName));
            //}

            return new AlterContainerContentsResultsMessage { Succeeded = succeeded, Output = "MWCTODO: get entity names working first" };
        }
    }
}
