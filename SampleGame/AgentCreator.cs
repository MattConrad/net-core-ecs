using System.Linq;
using System.Collections.Generic;
using EntropyEcsCore;

namespace SampleGame
{
    /// <summary>
    /// Baby's first entity creator. An agent is an entity that has:
    /// PhysicalObject, EntityName, Container (Inventory), and Faction.
    /// For now, agent is functionally equivalent to "combatant".
    /// </summary>
    internal static class AgentCreator
    {
        internal static long Agent(EcsRegistrar rgs, string agentBlueprintName, string armorBlueprintName, string weaponBlueprintName)
        {
            long agentId = Blueprinter.GetEntityFromBlueprint(rgs, agentBlueprintName);
            long armorId = Blueprinter.GetEntityFromBlueprint(rgs, armorBlueprintName);
            long weaponId = Blueprinter.GetEntityFromBlueprint(rgs, weaponBlueprintName);

            Sys.Equipment.WieldWeapon(rgs, agentId, weaponId, true);

            //WMCTODO: we haven't fixed armor yet!

            //packPart.EntityIds.Add(armorId);
            //packPart.EntityIds.Add(weaponId);

            return agentId;
        }
    }

}
