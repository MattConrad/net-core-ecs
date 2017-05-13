using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Messages
{
    internal class Combat
    {
        internal long EventId { get; set; }
        internal string EventType { get; set; }
        internal long ActorId { get; set; }
        internal Parts.EntityName ActorNames { get; set; }
        internal long RecipientId { get; set; }
        internal Parts.EntityName RecipientName { get; set; }
    }
}
