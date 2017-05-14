using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    public class GameValueString
    {
        private string _val;

        public GameValueString(string val)
        {
            _val = val;
        }

        public static implicit operator GameValueString(string val)
        {
            if (val == null) return null;

            return new GameValueString(val);
        }
    }
}
