using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sentinel.Validator.POC.Repo
{
    public class DictionaryMemoryRepo<TValue> : Dictionary<string, TValue>, IDictionaryRepo<TValue>
    {
        public DictionaryMemoryRepo(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; set; }

        public void Add(TValue value) => Add(PropertyInfoHelpers.GetKeyValue<string, TValue>(value), value);

        public void AddMultiple(IEnumerable<TValue> values)
        {
            foreach (var item in values)
            {
                Add(item);
            }
        }

        public void AddMultipleSync(IEnumerable<KeyValuePair<string, TValue>> values)
        {
            foreach (var item in values)
            {
                Add(item.Key, item.Value);
            }
        }

        public Task AddSync(TValue value) => Task.Run(() => this.Add(PropertyInfoHelpers.GetKeyValue<string, TValue>(value), value));

        public bool ContainsKey(TValue value) => ContainsKey(PropertyInfoHelpers.GetKeyValue<string, TValue>(value));

        public bool Remove(TValue value) => Remove(PropertyInfoHelpers.GetKeyValue<string, TValue>(value));


        public void Sync(IEnumerable<TValue> values, bool overrideExisting = true)
        {
            var keys = Keys;
            foreach (var item in values)
            {
                if (overrideExisting)
                {
                    Add(item);
                }
                else
                {
                    string? itemKey = PropertyInfoHelpers.GetKeyValue<string, TValue>(item);
                    if (!keys.Any(p => p == itemKey))
                    {
                        Add(item);
                    }
                }
            }

            foreach (var key in Keys)
            {
                if (!values.Any(p => PropertyInfoHelpers.GetKeyValue<string, TValue>(p) == key))
                {
                    Remove(key);
                }
            }
        }

        public void Sync(IEnumerable<KeyValuePair<string, TValue>> values, bool overrideExisting = true)
        {
            var keys = Keys;
            foreach (var item in values)
            {

                if (!keys.Any(p => p == item.Key))
                {
                    Add(item.Key, item.Value);
                }
            }

            foreach (var key in Keys)
            {
                if (!values.Any(p => p.Key == key))
                {
                    Remove(key);
                }
            }
        }

        public void Sync(IEnumerable<TValue> values, Func<TValue, string> keySelector)
        {
            var keys = Keys;
            foreach (var item in values)
            {
                var key = keySelector.Invoke(item);
                if (!keys.Any(p => p == key))
                {
                    Add(key, item);
                }
            }

            foreach (var key in Keys)
            {
                if (!values.Any(p => keySelector.Invoke(p) == key))
                {
                    Remove(key);
                }
            }
        }
    }
}