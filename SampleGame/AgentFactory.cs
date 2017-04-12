using System.Linq;
using System.Collections.Generic;
using EntropyEcsCore;

namespace SampleGame
{

    /// <summary>
    /// Baby's first entity factory. An agent is an entity that has:
    /// PhysicalObject, EntityName, Container (Inventory), Container (Equipment), and Faction.
    /// For now, agent is functionally equivalent to "combatant".
    /// </summary>
    internal static class AgentFactory
    {
        internal static long CreateAgent(EcsRegistrar rgs)
        {
            long agentId = rgs.CreateEntity();

            //MWCTODO: should seriously consider making a constructor for various components rather than using the constants all over the place
            rgs.AddComponent(agentId, nameof(CpPhysicalObject), new Dictionary<string, object>
            {
                [CpPhysicalObject.Keys.Condition] = CpPhysicalObject.Vals.Condition.Undamaged,
                [CpPhysicalObject.Keys.Size] = CpPhysicalObject.Vals.Size.Medium
            });

            rgs.AddComponent(agentId, nameof(CpEntityName), new Dictionary<string, object>
            {
                [CpEntityName.Keys.ProperName] = "",
                [CpEntityName.Keys.VagueName] = "orc",
                [CpEntityName.Keys.ShortDescription] = "The hideous orc.",
                [CpEntityName.Keys.LongDescription] = "A hideous orc, short but muscular, missing an ear. You suspect he wants to claim your ears as a replacement."
            });

            return agentId;
        }
    }

}
