using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntropyEcsCore;
using GM = SampleGame.MessageDefs;
using GCp = SampleGame.ComponentDefs;

namespace SampleGame
{
    //in the CoQ talk, everything was handled via events. that seems awfully confusing. for now, let's stick with systems that call each other in fixed ways.
    //public static class Dispatcher
    //{
    //    public static DataDict DispatchMessage(EcsRegistrar rgs, long originEntityId, string messageType, DataDict data)
    //    {
    //        if (messageType == GM.CombatMessages.Messages.Attack) return CombatSystem.ResolveAttack(rgs, data);
    //        if (messageType == "") return EntityNameSystem.GetNameInfo();
    //        if (messageType == "") return CombatSystem.ResolveAttack(rgs, data);

    //        return new DataDict();
    //    }
    //}

    public static class EntityNameSystem
    {
        public static DataDict GetEntityNames(EcsRegistrar rgs, long entityId)
        {
            return rgs.GetComponentsOfType(entityId, nameof(GCp.CpEntityName)).First().Data;
        }
    }

    public static class RandomSystem
    {
        private static Random _rand = new Random();

        /// <summary>
        /// Returns a random number in the range of -7/+7.
        /// </summary>
        public static int GetRange7()
        {
            //later, we'll turn this into a bell curve.
            return _rand.Next(15) - 7;
        }
    }

    public static class ContainerSystem
    {
        public enum Action
        {
            AddEntity,
            RemoveEntity
        }

        //eventually entities bearing containers may have weight, size, inventory count, or other limits. for now, simple.
        public static DataDict Add(EcsRegistrar rgs, long ownerId, long toAddId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toAddId, containerId);
        }

        public static DataDict Remove(EcsRegistrar rgs, long ownerId, long toRemoveId, long? containerId = null)
        {
            return AddOrRemove(Action.AddEntity, rgs, ownerId, toRemoveId, containerId);
        }

        public static DataDict AddOrRemove(Action action, EcsRegistrar rgs, long ownerId, long toAddOrRemoveId, long? containerId = null)
        {
            //we might have many containers for an entity later, in which case containerId or other filtering may matter.
            var container = rgs.GetComponentsOfType(ownerId, nameof(GCp.CpContainer)).First();
            //MWCTODO: oh, ids really want to be a HashSet
            var ids = container.Data.GetListLong(GCp.CpContainer.Keys.ContainedEntityIds);

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
            results[GM.AlterContainerContentsResultsMessage.Keys.Succeeded] = succeeded;

            var ownerName = rgs.GetComponentsOfType(ownerId, nameof(GCp.CpEntityName)).FirstOrDefault();
            var addedName = rgs.GetComponentsOfType(toAddOrRemoveId, nameof(GCp.CpEntityName)).FirstOrDefault();

            //probably, eventually, we want the container 
            if (ownerName != null && addedName != null)
            {
                string baseMessage = action == Action.AddEntity ? "Added {0} to {1}." : "Removed {0} from {1}.";

                results[nameof(GM.AlterContainerContentsResultsMessage.Keys.Output)] =
                    string.Format(baseMessage,
                        addedName.Data.GetString(GCp.CpEntityName.Keys.VagueName),
                        ownerName.Data.GetString(GCp.CpEntityName.Keys.VagueName));
            }

            return results;
        }
    }

    public static class CombatSystem
    {
        public static DataDict ResolveAttack(EcsRegistrar rgs, long attackerId, DataDict attackData)
        {
            var targetType = attackData.GetString(GM.AttackMessage.Keys.TargetType);
            if (targetType != GM.AttackMessage.Vals.TargetType.SingleMelee) throw new ArgumentException("Melee only.");

            int roll = RandomSystem.GetRange7();

            long targetId = attackData.GetLong(GM.AttackMessage.Keys.TargetEntityId);



            return attackData;
        }
    }

    public static class ConsoleSystem
    {
        public static DataDict DisplayLines(DataDict linesDict)
        {
            var lines = linesDict.GetListString(GM.ConsoleMessage.Keys.Lines);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            return linesDict;
        }

    }

}
