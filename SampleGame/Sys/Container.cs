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

        internal static long CreateBattlefield(EcsRegistrar rgs, IEnumerable<long> battlefieldEntityIds)
        {
            var battlefieldId = rgs.CreateEntity();
            var battlefieldContainer = new Parts.Container { Description = "battlefield", Tag = Vals.ContainerTag.Battlefield, ItemsAreVisible = true, Preposition = "on" };

            battlefieldEntityIds = battlefieldEntityIds ?? new long[] { };
            foreach(long entityId in battlefieldEntityIds)
            {
                battlefieldContainer.EntityIds.Add(entityId);
            }

            rgs.AddPart(battlefieldId, battlefieldContainer);

            return battlefieldId;
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

        internal static HashSet<long> GetEntityIdsFromFirstTagged(EcsRegistrar rgs, long ownerId, string tag)
        {
            var container = rgs.GetParts<Parts.Container>(ownerId).FirstOrDefault(c => c.Tag == tag);

            return container?.EntityIds ?? new HashSet<long>();
        }

        internal static AlterContainerContentsResultsMessage AddOrRemove(Action action, EcsRegistrar rgs, long ownerId, long itemAddOrRemoveId)
        {
            //MWCTODO: we might have many containers for an entity later, so this is temporary only.
            var containerPart = rgs.GetParts<Parts.Container>(ownerId).FirstOrDefault();

            if (containerPart == null) return new AlterContainerContentsResultsMessage { Succeeded = false, Output = "Container not found." };

            var ids = containerPart.EntityIds;

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

            var ownerNamePart = rgs.GetParts<Parts.EntityName>(ownerId).Single();
            var itemNamePart = rgs.GetParts<Parts.EntityName>(itemAddOrRemoveId).SingleOrDefault();

            string verbPhrase = (action == Action.AddEntity)
                ? $"adds {itemNamePart.GeneralName} to"
                : $"removes {itemNamePart.GeneralName} from";

            string actionPhrase = $"{ownerNamePart.ProperName} {verbPhrase} {ownerNamePart.Pronoun} {containerPart.Tag}.";

            return new AlterContainerContentsResultsMessage { Succeeded = true, Output = actionPhrase };
        }
    }
}
