﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Messages
{
    //MWCTODO: we'll want history eventually, should this maybe be a combat history part that is added successively to the battlefield?
    // NOTE: if we make it into a part, can't have properties that are parts themselves.

    /// <summary>
    /// A combat action might have multiple targets, so there might be multiple combat messages from the same action.
    /// </summary>
    internal class Combat
    {
        internal long Tick { get; set; }
        internal long ActorId { get; set; }
        internal long TargetId { get; set; }
        internal string ActorAction { get; set; }
        internal string TargetAction { get; set; }
        internal string ActorCritical { get; set; }
        internal string TargetCritical { get; set; }
        internal int ActorAdjustedSkill { get; set; }
        internal int TargetAdjustedSkill { get; set; }
        internal int ActorAdjustedRoll { get; set; }
        internal int TargetAdjustedRoll { get; set; }
        internal int NetActorRoll { get; set; }
        internal List<string> NewActorCombatTags { get; set; } = new List<string>();
        internal List<string> NewTargetCombatTags { get; set; } = new List<string>();
        //dunno if we should dupe this info here, it's on the entity as well.
        internal long TargetCondition { get; set; }
        internal string PrimaryDamageType { get; set; }
        internal string PrimaryDamagePreventer { get; set; }
        internal decimal AttemptedDamage { get; set; }
        //MWCTODO+: these two are wrong, they have to fit the dam resistance model better.
        internal decimal DefaultPreventedDamage { get; set; }
        internal decimal ProtectionPreventedDamage { get; set; }

        internal decimal NetDamage { get; set; }
    }

}
