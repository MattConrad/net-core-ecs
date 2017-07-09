using System;

using System.Collections.Generic;
using System.Linq;

namespace SampleGame.Sys
{
    /*
     * System.Random is a bit bugged: https://connect.microsoft.com/VisualStudio/feedback/details/634761/system-random-serious-bug#tabs 
     * could use reflector and fix, or maybe https://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
     */
    internal class Rando
    {
        private static Random _rand = new Random();

        /// <summary>
        /// Returns a random number in the range of 0 - 7, and also -7, with +/- 7 being very rare, 
        /// 0 being most common, and increasing up to 6 being increasingly less common.
        /// </summary>
        internal static int GetRange7()
        {
            int baseVal = Get1To100();

            if (baseVal == 100) return 7;
            if (baseVal >= 92) return 6;
            if (baseVal >= 82) return 5;
            if (baseVal >= 70) return 4;
            if (baseVal >= 56) return 3;
            if (baseVal >= 40) return 2;
            if (baseVal >= 22) return 1;
            if (baseVal >= 2) return 0;
            return -7;
        }

        internal static int Get1To100()
        {
            return _rand.Next(100) + 1;
        }
    }
}
