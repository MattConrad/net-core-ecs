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
            return new AgentBundle
            {
                Agent = rgs.GetPartSingle<Parts.Agent>(agentId),
                Anatomy = rgs.GetPartSingle<Parts.Anatomy>(agentId),
                EntityName = rgs.GetPartSingle<Parts.EntityName>(agentId),
                PhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(agentId)
            };
        }

        internal static WeaponBundle GetWeaponBundle(EcsRegistrar rgs, long weaponId)
        {
            return new WeaponBundle
            {
                EntityName = rgs.GetPartSingle<Parts.EntityName>(weaponId),
                PhysicalObject = rgs.GetPartSingle<Parts.PhysicalObject>(weaponId),
                Damagers = rgs.GetParts<Parts.Damager>(weaponId).ToList()
            };
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
