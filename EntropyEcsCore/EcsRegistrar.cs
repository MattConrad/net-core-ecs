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
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
        }

        public EcsRegistrar(string serializedRegistry) : this()
        {
            _entityIdsToEntityParts = JsonConvert.DeserializeObject<Dictionary<long, List<EcsEntityPart>>>(serializedRegistry);
        }

        /// <summary>
        /// This can be used for both entities and parts.
        /// </summary>
        public long NewId()
        {
            return _currentId++;
        }

        /// <summary>
        /// Add an entity part to an entity. If the part doesn't have an Id yet, a new Id will be assigned.
        /// </summary>
        public void AddPart(long entityId, EcsEntityPart cp)
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
        /// Loads a new entity into the registry from "simple" serialized parts, returning new entity Id.
        /// Automatically assigns new, valid Ids for each deserialized part, ignoring previous serialized Id.
        /// Works on single-level, simple part sets only. Any part containing a id reference to 
        /// other entities or parts will NOT work here, and will corrupt the registry.
        /// </summary>
        public long CreateEntity(string serializedEntityParts)
        {
            long entityId = NewId();

            var entityParts = DeserializeParts(serializedEntityParts);
            foreach(var part in entityParts)
            {
                part.Id = NewId();
            }

            _entityIdsToEntityParts.Add(entityId, entityParts);

            return entityId;
        }

        public List<EcsEntityPart> DeserializeParts(string serializedEntityParts)
        {
            return JsonConvert.DeserializeObject<List<EcsEntityPart>>(serializedEntityParts);
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

        public EcsEntityPart GetPartById(long entityId, long partId)
        {
            return _entityIdsToEntityParts[entityId].Single(cp => cp.Id == partId);
        }

        /// <summary>
        /// Serialize entity parts, leaving all Ids as-is.
        /// </summary>
        public string SerializeEntityParts(long entityId)
        {
            return SerializeEntityParts(_entityIdsToEntityParts[entityId]);
        }

        /// <summary>
        /// Serialize entity parts, leaving all Ids as-is.
        /// </summary>
        public string SerializeEntityParts(List<EcsEntityPart> entityParts)
        {
            return JsonConvert.SerializeObject(entityParts);
        }

        /// <summary>
        /// Serialize entire registry, leaving all Ids as-is.
        /// </summary>
        public string SerializeRegistry()
        {
            return JsonConvert.SerializeObject(_entityIdsToEntityParts);
        }

    }
}
