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

        //MWCTODO: we're going to need some way of polling the entities to see what actions are available for a given entity.
        // this might be a good place for an interface, because lots of different types of parts might want to weigh in on that question.

        public string DefaultCombatAI { get; set; }
        public string ActiveCombatAI { get; set; }

        public static class Vals
        {
            public static class AI
            {
                public const string PlayerChoice = nameof(PlayerChoice);
                public const string MeleeOnly = nameof(MeleeOnly);
            }
        }
    }
}
