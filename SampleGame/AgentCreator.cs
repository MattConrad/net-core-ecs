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

            var packPart = rgs.GetParts<Parts.Container>(agentId).Single(p => p.Tag == Vals.ContainerTag.Equipped);
            packPart.EntityIds.Add(armorId);
            packPart.EntityIds.Add(weaponId);

            return agentId;
        }

        //internal static long Agent(EcsRegistrar rgs, string properName, string generalName, string shortDescription, 
        //    IEnumerable<long> inventoryIds, string inventoryContainerDescription, int meleeSkill,
        //    string factionName)
        //{
        //    long agentId = rgs.CreateEntity();

        //    rgs.AddPart(agentId, new Parts.PhysicalObject());
        //    rgs.AddPart(agentId, new Parts.EntityName { ProperName = properName, GeneralName = generalName, ShortDescription = shortDescription });
        //    rgs.AddPart(agentId, new Parts.Skillset { Skills = new Dictionary<string, int> { [Parts.Skillset.Vals.Physical.Melee] = meleeSkill } });

        //    rgs.AddPart(agentId, new Parts.Container() {
        //        ContainedEntityIds = new HashSet<long>(inventoryIds ?? new long[] { }), ContainerDescription = inventoryContainerDescription
        //    });
        //    rgs.AddPart(agentId, new Parts.Faction { FactionReputations = new Dictionary<string, int> { [factionName] = 100 } });

        //    return agentId;
        //}
    }

}
