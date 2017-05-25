using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Vals
{
    //can we spec the universe with less than ten sizes? maybe.
    public static class Size
    {
        public static Dictionary<string, int> SizeToInt = new Dictionary<string, int>
        {
            [Gnat] = 0,
            [Rabbit] = 1,
            [Gnome] = 2,
            [Human] = 3,
            [Goliath] = 4,
            [StormGiant] = 5,
            [Smaug] = 6,
            [Mountain] = 7
        };

        public const string Gnat = nameof(Gnat);
        public const string Rabbit = nameof(Rabbit);
        public const string Gnome = nameof(Gnome);
        public const string Human = nameof(Human);
        public const string Goliath = nameof(Goliath);
        public const string StormGiant = nameof(StormGiant);
        public const string Smaug = nameof(Smaug);
        public const string Mountain = nameof(Mountain);
        public const string Special = nameof(Special);
        public const string None = nameof(None);
    }

}
