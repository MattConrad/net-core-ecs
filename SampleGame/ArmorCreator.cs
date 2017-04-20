using System;
using System.Collections.Generic;
using System.Text;
using EntropyEcsCore;

namespace SampleGame
{
    public static class ArmorCreator
    {
        internal static long Armor(EcsRegistrar rgs, string generalName, string shortDescription, long defenseValue)
        {
            long agentId = rgs.CreateEntity();

            rgs.AddComponent(agentId, CpPhysicalObject.Create());
            rgs.AddComponent(agentId, CpEntityName.Create(properName: "", generalName: generalName, shortDescription: shortDescription));

            return agentId;
        }

    }
}
