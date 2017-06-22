using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleGame
{
    public static class Extensions
    {
        //nice approach from (surprise) Jon Skeet: https://stackoverflow.com/questions/2601477/dictionary-returning-a-default-value-if-the-key-does-not-exist
        //omitting the func overload (is ambiguous if null is default) but note that it does exist.

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        // https://stackoverflow.com/questions/4171140/iterate-over-values-in-flags-enum

        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }



}
