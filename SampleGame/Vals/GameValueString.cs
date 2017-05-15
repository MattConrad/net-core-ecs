using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public class GameValueString
    {
        private string _string;

        public GameValueString(string val)
        {
            _string = val;
        }

        public static implicit operator String(GameValueString gvString)
        {
            if (gvString == null) return null;

            return gvString._string;
        }

        public static implicit operator GameValueString(string sysString)
        {
            if (sysString == null) return null;

            return new GameValueString(sysString);
        }
    }
}
