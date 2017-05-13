using System;
using System.Collections.Generic;

namespace SampleNarrator
{
    public class Message
    {
        public long EventId { get; set; }
        public EventCategory Category { get; set; }
        public long ActorId { get; set; }
        public long RecipientId { get; set; }
        //we want a lot more detail here. maybe a dictionary, but having learned my lession, maybe classes implementing a marker interface. not-a-sure.
        public string ActorProperName { get; set; }
        public string RecipientProperName { get; set; }
    }

    //mmmm, i dunno. i doubt i'll keep this. no i really don't like. 
    public enum EventCategory
    {
        CombatAttack = 0,
        CombatCriticalAmazing = 1,
        CombatCriticalExcellent = 2,
        CombatCriticalPoor = 3,
        CombatCriticalTerrible = 4,
        CombatDodge = 5,
        CombatFatality = 6 
    }

}
