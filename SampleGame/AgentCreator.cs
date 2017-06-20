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

            var anatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId);

            if (!anatomy.SlotsEquipped.Any())
            {
                anatomy.SlotsEquipped = GetSlotsEquippedForBodyPlan(anatomy.BodyPlan);
            }

            var wieldResults = Sys.Anatomy.WieldWeapon(rgs, agentId, weaponId);
            var armorResults = Sys.Anatomy.NewEquip(rgs, agentId, armorId);

            return agentId;
        }

        //this wants a better home eventually.
        private static List<KeyValuePair<string, long>> GetSlotsEquippedForBodyPlan(string bodyPlan)
        {
            return Vals.BodyPlan.OldBodyPlanSlots[bodyPlan].Select(s => new KeyValuePair<string, long>(s, 0)).ToList();
        }

    }

}
