using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace EntropyEcsCore
{

    //http://stackoverflow.com/questions/42495525/restricting-value-types-in-a-c-sharp-dictionary was me, but i don't think John's approach is good for as many types as I want here.

    //MWCTODO: pretty sure we want to support Guid and maybe List<Guid> as a value type here also. is this manually coded type checking really helping?

    /// <summary>
    /// Dictionary class with string key that only accepts values of certain supported types, and has convenience methods for accessing methods of these types.
    /// </summary>
    public class DataDict
    {
        /// <summary>
        /// Return a new DataDict from any untyped string:object dictionary. Throws exception if any invalid values.
        /// </summary>
        public static DataDict GetDataDict(Dictionary<string, object> untypedDict)
        {
            var dataDict = new DataDict();

            foreach (string key in untypedDict.Keys)
            {
                dataDict[key] = untypedDict[key];
            }

            return dataDict;
        }

        private Dictionary<string, object> _internalDict = new Dictionary<string, object>();

        private bool IsValidValue(object value)
        {
            if (value is Dictionary<string, object>)
            {
                return ((Dictionary<string, object>)value).Values.All(IsValidValue);
            }

            //for now, we won't support IEnumerable<T> as values, only List<T>s. maybe arrays, if we end up rehydrating json or something.
            return (value is long || value is string || value is decimal || value is bool
                    || value is List<long> || value is List<string> || value is List<decimal>
                    || value is HashSet<long>);
        }

        public object this[string key]
        {
            //for now, we won't support retrieving objects with the indexer. you're supposed to know the type of object you're getting.
            set
            {
                if (IsValidValue(value))
                {
                    _internalDict[key] = value;
                }
                else
                {
                    throw new InvalidOperationException("Unsupported DataDict value type. Cannot set value.");
                }
            }
        }

        public void Add(string key, long value)
        {
            _internalDict.Add(key, value);
        }

        public void Add(string key, string value)
        {
            _internalDict.Add(key, value);
        }

        public void Add(string key, decimal value)
        {
            _internalDict.Add(key, value);
        }

        public void Add(string key, bool value)
        {
            _internalDict.Add(key, value);
        }

        public void Add(string key, IEnumerable<long> value)
        {
            _internalDict.Add(key, value.ToList());
        }

        public void Add(string key, IEnumerable<string> value)
        {
            _internalDict.Add(key, value.ToList());
        }

        public void Add(string key, IEnumerable<decimal> value)
        {
            _internalDict.Add(key, value.ToList());
        }

        public void Add(string key, DataDict value)
        {
            _internalDict.Add(key, value);
        }

        public void AppendString(string key, string value)
        {
            var obj = _internalDict[key];
            ((List<string>)obj).Add(value);
        }

        //we probably want Append or AddRange for the IEnumerables, but let's see what happens.
        public void AppendLong(string key, long value)
        {
            var obj = _internalDict[key];

            if (obj is List<long>)
            {
                ((List<long>)obj).Add(value);
            }
            else if (obj is HashSet<long>)
            {
                ((HashSet<long>)obj).Add(value);
            }
            else
            {
                throw new InvalidOperationException($"Value for key '{key}' cannot append long value.");
            }
        }

        public bool ContainsKey(string key)
        {
            return _internalDict.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            return (T)_internalDict[key];
        }

        public long GetLong(string key)
        {
            return (long)_internalDict[key];
        }

        public string GetString(string key)
        {
            return (string)_internalDict[key];
        }

        public decimal GetDecimal(string key)
        {
            return (decimal)_internalDict[key];
        }

        public bool GetBool(string key)
        {
            return (bool)_internalDict[key];
        }

        public HashSet<long> GetHashSetLong(string key)
        {
            return (HashSet<long>)_internalDict[key];
        }

        public List<long> GetListLong(string key)
        {
            return (List<long>)_internalDict[key];
        }

        public List<string> GetListString(string key)
        {
            return (List<string>)_internalDict[key];
        }

        public List<decimal> GetListDecimal(string key)
        {
            return (List<decimal>)_internalDict[key];
        }

        public DataDict GetDataDict(string key)
        {
            return (DataDict)_internalDict[key];
        }

        //we might need TryGet or maybe GetDefault also, but let's see what happens.

        public Dictionary<string, object>.KeyCollection Keys()
        {
            return _internalDict.Keys;
        }

        public bool Remove(string key)
        {
            return _internalDict.Remove(key);
        }

    }
}
