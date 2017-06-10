using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public static class Switchboard
    {
        //i could see this becoming a separate project eventually.
        // if so, actor choice categories would need to move here from vals. also combatcategory (which is poorly named anyway).
    }

    public class CombatChoicesAndTargets
    {
        public List<AgentActionChoice> Choices { get; set; } = new List<AgentActionChoice>();
        public Dictionary<long, string> MeleeTargets { get; set; } = new Dictionary<long, string>();
        public Dictionary<long, string> RangedTargets { get; set; } = new Dictionary<long, string>();
    }

    //MWCTODO: this is kind of a mess. it includes actions at all different "levels", sometimes it has Action and sometimes not, 
    //  it includes actions with and without weapons, with and without NextCategory.
    //  It is very likely inadequate for some future scenarios like spellcasting.
    //  I considered "normalizing" the the data organization more but I think that would add trade one kind of client-side complexity for a worse. 
    //  Anyway, this could use a bright idea or two.
    //Note the importance of "NextCategory". If this is null, then this choice is a terminal choice. If it is not-null, there will always be a submenu under the choice (or the choice is corrupt data).
    public class AgentActionChoice
    {
        private string _description = "(no description)";

        public long AgentId { get; set; }
        public string Category { get; set; }
        public string Action { get; set; }
        //eventually we'll want ExtendedDescription, since desc here is so short.
        public string Description {
            get { return _description; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Description cannot be blank.");
                if (value.Length > 50) throw new ArgumentException("Description must be 50 characters or less.");

                _description = value;
            }
        }
        public int? WeaponHandIndex { get; set; }
        public long? WeaponEntityId { get; set; }
        public string NextCategory { get; set; }
    }

    public class ActionChosen
    {
        public long AgentId { get; set; }
        public string Action { get; set; }
        public int? WeaponHandIndex { get; set; }
        public long? WeaponEntityId { get; set; }
        public long? TargetEntityId { get; set; }
    }

}
