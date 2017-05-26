using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public static class Extensions
    {
        //nice approach from (surprise) Jon Skeet: https://stackoverflow.com/questions/2601477/dictionary-returning-a-default-value-if-the-key-does-not-exist
        //omitting the func overload (is ambigious if null is default) but note that it does exist.

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
