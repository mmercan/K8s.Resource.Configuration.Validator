using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentinel.Validator.POC.Repo
{
    public interface IDictionaryRepo<TValue> : IDictionary<string, TValue>
    {
        bool ContainsKey(TValue value);
        bool Remove(TValue value);
        ILogger Logger { get; set; }
        void Add(TValue value);

        Task AddSync(TValue value);
        void AddMultiple(IEnumerable<TValue> values);
        void AddMultipleSync(IEnumerable<KeyValuePair<string, TValue>> values);
        void Sync(IEnumerable<TValue> values, bool overrideExisting = true);
        void Sync(IEnumerable<KeyValuePair<string, TValue>> values, bool overrideExisting = true);

        void Sync(IEnumerable<TValue> values, Func<TValue, string> keySelector);
    }
}