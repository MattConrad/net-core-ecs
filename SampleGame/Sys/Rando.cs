using System;

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
        /// Returns a random number in the range of -5/+5, with 0 most common and +/- 5 quite rare.
        /// </summary>
        internal static int GetRange5()
        {
            //half the time, we want to multiply results by -1.
            int sign = (_rand.Next(0, 2) == 0) ? 1 : -1;

            //this isn't math clever but it will do for a bellish curve.
            int baseVal = Get1To100();

            if (baseVal == 100) return 5 * sign;
            if (baseVal >= 95) return 4 * sign;
            if (baseVal >= 85) return 3 * sign;
            if (baseVal >= 65) return 2 * sign;
            if (baseVal >= 35) return 1 * sign;

            return 0;
        }

        internal static int Get1To100()
        {
            return _rand.Next(100) + 1;
        }
    }
}
