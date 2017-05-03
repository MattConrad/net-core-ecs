using System.Collections.Generic;
using System.IO;
using EntropyEcsCore;
//using Newtonsoft.Json;

namespace SampleGame
{
    /// <summary>
    /// This class serializes and deserializes SIMPLE entities as blueprints. It cannot handle any entity with references to other entity ids.
    /// </summary>
    internal class Blueprinter
    {
        private static string _blueprintFolder = System.AppContext.BaseDirectory + @"\Blueprints\";
        private static string _blueprintSuffix = ".json";

        /// <summary>
        /// Write (or overwrite) a blueprint for an entity.
        /// </summary>
        internal static void WriteBlueprint(EcsRegistrar rgs, long entityId, string blueprintName)
        {
            string revisedPartsJson = GetBlueprint(rgs, entityId);

            File.WriteAllText(_blueprintFolder + blueprintName + _blueprintSuffix, revisedPartsJson);
        }

        //as Parts evolve, the blueprints won't keep up. different ways we could handle, one might be to cycle through
        // every blueprint in the folder and load/rewrite it, that will at least get new props in as nulls.

        /// <summary>
        /// Get a blueprint for an entity. Blueprint part Ids are all forced to 0 (to reduce possible
        /// confusion when reading by hand). This method has a limited exception to the no-references 
        /// rule--an entity with Container parts will automatically have those Containers emptied 
        /// before writing (which makes them safe).
        /// </summary>
        internal static string GetBlueprint(EcsRegistrar rgs, long entityId)
        {
            //make a copy before changing anything, just in case the user wants to continue working with original.
            string originalPartsJson = rgs.SerializeEntityParts(entityId);
            var revisedPartsList = rgs.DeserializeParts(originalPartsJson);

            foreach (var part in revisedPartsList)
            {
                part.Id = 0;
                if (part is Parts.Container) ((Parts.Container)part).EntityIds.Clear();
            }

            return rgs.SerializeEntityParts(revisedPartsList);
        }

        internal static long GetEntityFromBlueprint(EcsRegistrar rgs, string blueprintName)
        {
            var blueprintJson = File.ReadAllText(_blueprintFolder + blueprintName + _blueprintSuffix);

            return rgs.CreateEntity(blueprintJson);
        }

    }
}
