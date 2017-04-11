using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    internal static class RandomSystem
    {
        private static Random _rand = new Random();

        /// <summary>
        /// Returns a random number in the range of -5/+5.
        /// </summary>
        internal static int GetRange5()
        {
            //MWCTODO: later, we'll turn this into a bell curve.
            return _rand.Next(11) - 5;
        }
    }
}
