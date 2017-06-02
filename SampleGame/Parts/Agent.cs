using System.Collections.Generic;

namespace SampleGame.Parts
{
    public class Agent : EntropyEcsCore.EcsEntityPart
    {
        //for now, an agent is an part that indicates the entity gets turns in combat.

        // we want to know if the entity is currently player controlled or AI controlled.
        // there should be a default AI for any agent (including the hero)
        // and also a current AI. AI will just be a defined string that indicates which method in the AI system
        // is used for action determination. i haven't thought about how those will work yet, but we can
        // start with basic melee only.

        public string DefaultCombatAI { get; set; }
        public string ActiveCombatAI { get; set; }

        public List<string> CombatStatusTags { get; set; } = new List<string>();
    }
}
