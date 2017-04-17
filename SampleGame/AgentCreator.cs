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
        internal static long Agent(EcsRegistrar rgs, string properName, string generalName, string shortDescription, 
            IEnumerable<long> inventoryIds, string inventoryContainerDescription,
            string factionName)
        {
            long agentId = rgs.CreateEntity();

            rgs.AddComponent(agentId, CpPhysicalObject.Create());
            rgs.AddComponent(agentId, CpEntityName.Create(properName: properName, generalName: generalName, shortDescription: shortDescription));
            rgs.AddComponent(agentId, CpContainer.Create(inventoryIds, inventoryContainerDescription));
            rgs.AddComponent(agentId, CpFaction.Create(factionName));

            return agentId;
        }
    }

}
