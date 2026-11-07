using System;
using System.Collections.Generic;

namespace LRH.Core
{
    public sealed class Registry<T> where T : class
    {
        private readonly Dictionary<string, T> items = new Dictionary<string, T>();
        public int Count => items.Count;
        public bool Register(string id, T value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (items.ContainsKey(id)) return false;
            items.Add(id, value);
            return true;
        }
        public bool TryGet(string id, out T value) => items.TryGetValue(id, out value);
        public bool Remove(string id) => items.Remove(id);
    }
}
