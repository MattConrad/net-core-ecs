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
            long? armorId = string.IsNullOrEmpty(armorBlueprintName) ? (long?)null : Blueprinter.GetEntityFromBlueprint(rgs, armorBlueprintName);
            long? weaponId = string.IsNullOrEmpty(weaponBlueprintName) ? (long?)null :  Blueprinter.GetEntityFromBlueprint(rgs, weaponBlueprintName);

            var anatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId);

            if (!anatomy.SlotsEquipped.Any())
            {
                var bodySlotsAsEnum = Vals.BodyPlan.BodyPlanToSlots[anatomy.BodyPlan];
                anatomy.SlotsEquipped = bodySlotsAsEnum.GetUniqueFlags().ToDictionary(s => (Vals.BodySlots)s, s => 0L);
            }

            if (armorId.HasValue)
            {
                var armorResults = Sys.Anatomy.Equip(rgs, agentId, armorId.Value);
            }

            if (weaponId.HasValue)
            {
                var wieldResults = Sys.Anatomy.Equip(rgs, agentId, weaponId.Value);
            }

            return agentId;
        }

        //this wants a better home eventually.
        private static List<KeyValuePair<string, long>> GetSlotsEquippedForBodyPlan(string bodyPlan)
        {
            throw new System.NotImplementedException();
            //return Vals.BodyPlan.OldBodyPlanSlots[bodyPlan].Select(s => new KeyValuePair<string, long>(s, 0)).ToList();
        }

    }

}
