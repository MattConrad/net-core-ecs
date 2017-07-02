using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class Bundle
    {
        internal static AgentBundle GetAgentBundle(EcsRegistrar rgs, long agentId)
        {
            var bundle = new AgentBundle();

            bundle.Agent = rgs.GetPartSingle<Parts.Agent>(agentId);
            bundle.Anatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId);
            bundle.EntityName = rgs.GetPartSingle<Parts.EntityName>(agentId);
            bundle.PhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(agentId);

            return bundle;
        }

        internal static WeaponBundle GetWeaponBundle(EcsRegistrar rgs, long weaponId)
        {
            var bundle = new WeaponBundle();

            bundle.EntityName = rgs.GetPartSingle<Parts.EntityName>(weaponId);
            bundle.PhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(weaponId);
            bundle.Damagers = rgs.GetParts<Parts.Damager>(weaponId).ToList();
 
            return bundle;
        }
    }

    internal class WeaponBundle
    {
        internal Parts.EntityName EntityName { get; set; }
        internal Parts.PhysicalObject PhysicalObject { get; set; }
        internal List<Parts.Damager> Damagers { get; set; }
    }


    internal class AgentBundle
    {
        internal Parts.Agent Agent { get; set; }
        internal Parts.Anatomy Anatomy { get; set; }
        internal Parts.EntityName EntityName { get; set; }
        internal Parts.PhysicalObject PhysicalObject { get; set; }
    }
}
