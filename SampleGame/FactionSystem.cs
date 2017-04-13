using System;
using System.Collections.Generic;
using System.Text;
using EntropyEcsCore;

namespace SampleGame
{
    public class CpFaction
    {
        internal static class Keys
        {
            //MWCTODO: later, this becomes a dict of faction names with a reputation number for each faction.
            // for now, just one name for your primary faction, and you're automatically fully paid up with that faction.

            /// <summary>
            /// String:enum.
            /// </summary>
            public const string FactionName = nameof(FactionName);
        }

        internal static class Vals
        {
            internal static class FactionName
            {
                public const string Heroes = nameof(Heroes);
                public const string Villians = nameof(Villians);
            }
        }

        internal static EcsComponent Create(string factionName = Vals.FactionName.Villians)
        {
            var cp = new EcsComponent { Type = nameof(CpEntityName), Data = new DataDict() };

            cp.Data[Keys.FactionName] = factionName;

            return cp;
        }
    }


}
