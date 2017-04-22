using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EntropyEcsCore
{
    public class EcsRegistrar
    {
        private long _currentId = 0L;
        private Dictionary<long, List<EcsEntityPart>> _entityIdsToEntityParts = new Dictionary<long, List<EcsEntityPart>>();

        public EcsRegistrar()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public EcsRegistrar(string serializedRegistry) : this()
        {
            _entityIdsToEntityParts = JsonConvert.DeserializeObject<Dictionary<long, List<EcsEntityPart>>>(serializedRegistry);
        }

        /// <summary>
        /// This can be used for both entities and components.
        /// </summary>
        public long NewId()
        {
            return _currentId++;
        }

        /// <summary>
        /// Add an entity part to an entity. If the part doesn't have an Id yet, a new Id will be assigned.
        /// </summary>
        public void AddComponent(long entityId, EcsEntityPart cp)
        {
            if (cp.Id == 0L) cp.Id = NewId();

            _entityIdsToEntityParts[entityId].Add(cp);
        }

        /// <summary>
        /// Loads a new entity into the registry with an empty part list.
        /// </summary>
        public long CreateEntity()
        {
            long id = NewId();
            _entityIdsToEntityParts.Add(id, new List<EcsEntityPart>());

            return id;
        }

        /// <summary>
        /// Loads a new entity into the registry from serialized parts, returning new entity Id.
        /// </summary>
        public long CreateEntity(string serializedEntityParts)
        {
            var entityParts = JsonConvert.DeserializeObject<List<EcsEntityPart>>(serializedEntityParts);

            long id = NewId();
            _entityIdsToEntityParts.Add(id, entityParts);

            return id;
        }

        public void DestroyEntity(long entityId)
        {
            _entityIdsToEntityParts.Remove(entityId);
        }

        public List<EcsEntityPart> GetAllParts(long entityId)
        {
            return _entityIdsToEntityParts[entityId];
        }

        public IEnumerable<T> GetPartsOfType<T>(long entityId) where T : EcsEntityPart
        {
            return GetAllParts(entityId).OfType<T>();
        }

        public EcsEntityPart GetComponentById(long entityId, long componentId)
        {
            return _entityIdsToEntityParts[entityId].Single(cp => cp.Id == componentId);
        }

        public string SerializeEntityParts(long entityId)
        {
            return JsonConvert.SerializeObject(_entityIdsToEntityParts[entityId]);
        }

        public string SerializeRegistry()
        {
            return JsonConvert.SerializeObject(_entityIdsToEntityParts);
        }

    }
}
