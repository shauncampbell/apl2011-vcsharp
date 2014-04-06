using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APL
{
    class HashMap<TKey, TValue>
    {
        private List<KeyValuePair<TKey, TValue>> _values = new List<KeyValuePair<TKey, TValue>>();

        public void Add(TKey key, TValue value)
        {
            if (!this.ContainsKey(key))
            {
                _values.Add(new KeyValuePair<TKey,TValue>(key,value));
            }
        }

        public Boolean ContainsKey(TKey key)
        {
            List<KeyValuePair<TKey,TValue>> values = _values.FindAll(delegate(KeyValuePair<TKey, TValue> k) { return k.Key.Equals(key); });
            return (values.Count > 0);
        }

        public Boolean Contains(TValue value)
        {
            List<KeyValuePair<TKey, TValue>> values = _values.FindAll(delegate(KeyValuePair<TKey, TValue> k) { return k.Value.Equals(value); });
            return (values.Count > 0);
        }

        public TValue Item(TKey key)
        {
            return _values.Find(delegate(KeyValuePair<TKey, TValue> k) { return k.Key.Equals(key); }).Value;
        }
        
    }
}
