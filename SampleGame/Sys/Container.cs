using System.Collections.Generic;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Container
    {
        //MWCTODO: this probably dies, but we're still figuring this section out.
        internal class AlterContainerContentsResultsMessage
        {
            public bool Succeeded { get; set; }
            public string Output { get; set; }
        }

        public enum Action
        {
            AddEntity,
            RemoveEntity
        }

        //eventually entities bearing containers may have weight, size, inventory count, or other limits. for now, simple.
        internal static AlterContainerContentsResultsMessage Add(EcsRegistrar rgs, long ownerId, long toAddId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toAddId);
        }

        internal static AlterContainerContentsResultsMessage Remove(EcsRegistrar rgs, long ownerId, long toRemoveId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toRemoveId);
        }

        //MWCTODO: this probably isn't good, since containerDescription isn't even a defined game string at the moment.
        internal static HashSet<long> GetEntityIdsFromFirstContainerByDesc(EcsRegistrar rgs, long ownerId, string containerDescription)
        {
            var container = rgs.GetPartsOfType<Parts.Container>(ownerId).FirstOrDefault(c => c.ContainerDescription == containerDescription);

            return container?.ContainedEntityIds ?? new HashSet<long>();
        }

        internal static AlterContainerContentsResultsMessage AddOrRemove(Action action, EcsRegistrar rgs, long ownerId, long itemAddOrRemoveId)
        {
            //MWCTODO: we might have many containers for an entity later, so this is temporary only.
            var containerPart = rgs.GetPartsOfType<Parts.Container>(ownerId).FirstOrDefault();

            if (containerPart == null) return new AlterContainerContentsResultsMessage { Succeeded = false, Output = "Container not found." };

            var ids = containerPart.ContainedEntityIds;

            if (action == Action.AddEntity && !ids.Contains(itemAddOrRemoveId))
            {
                ids.Add(itemAddOrRemoveId);
            }
            else if (action == Action.RemoveEntity && ids.Contains(itemAddOrRemoveId))
            {
                ids.Remove(itemAddOrRemoveId);
            }
            else
            {
                return new AlterContainerContentsResultsMessage { Succeeded = false, Output = "Can't add what already exists, or remove what does not exist." };
            }

            var ownerNamePart = rgs.GetPartsOfType<Parts.EntityName>(ownerId).Single();
            var itemNamePart = rgs.GetPartsOfType<Parts.EntityName>(itemAddOrRemoveId).SingleOrDefault();

            string verbPhrase = (action == Action.AddEntity)
                ? $"adds {itemNamePart.GeneralName} to"
                : $"removes {itemNamePart.GeneralName} from";

            string actionPhrase = $"{ownerNamePart.ProperName} {verbPhrase} {ownerNamePart.Pronoun} {containerPart.ContainerDescription}.";

            return new AlterContainerContentsResultsMessage { Succeeded = true, Output = actionPhrase };
        }
    }
}
