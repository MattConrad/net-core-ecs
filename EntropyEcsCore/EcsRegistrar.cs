using System;
using System.Collections.Generic;
using System.Linq;

namespace EntropyEcsCore
{
    //NOT threadsafe, don't try to access same EcsRegistrar concurrently.
    //one entity registrar per game. if only a single game per environment, only need one registrar.
    public class EcsRegistrar
    {
        //http://ripplega.me/development/ecs-ez/ is talking to me at a level that is easy to understand.

        private long _currentId = 0L;
        private Dictionary<long, List<EcsComponent>> _entityIdsToComponents = new Dictionary<long, List<EcsComponent>>();

        /// <summary>
        /// This can be used for both entities and components.
        /// </summary>
        public long NewId()
        {
            return _currentId++;
        }

        public void AddComponent(long entityId, string type)
        {
            AddComponent(entityId, type, new DataDict());
        }

        public void AddComponent(long entityId, string type, DataDict data)
        {
            var cp = new EcsComponent { Id = NewId(), Type = type, Data = data };

            _entityIdsToComponents[entityId].Add(cp);
        }

        public void AddComponent(long entityId, EcsComponent cp)
        {
            if (cp.Id != 0) throw new ArgumentException("Component may not already have an id before adding.");
            if (string.IsNullOrWhiteSpace(cp.Type)) throw new ArgumentException("Component must have a type before adding.");

            cp.Id = NewId();
            _entityIdsToComponents[entityId].Add(cp);
        }

        public long CreateEntity()
        {
            long id = NewId();
            _entityIdsToComponents.Add(id, new List<EcsComponent>());

            return id;
        }

        public void DestroyEntity(long entityId)
        {
            _entityIdsToComponents.Remove(entityId);
        }

        public List<EcsComponent> GetAllComponents(long entityId)
        {
            return _entityIdsToComponents[entityId];
        }

        public IEnumerable<EcsComponent> GetComponentsOfType(long entityId, string componentType)
        {
            return GetAllComponents(entityId).Where(cp => cp.Type == componentType);
        }

        public EcsComponent GetComponent(long entityId, long componentId)
        {
            return _entityIdsToComponents[entityId].Single(cp => cp.Id == componentId);
        }
    }
}
