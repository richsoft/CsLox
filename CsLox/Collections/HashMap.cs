using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Collections
{
    /// <summary>
    /// HashMap to add Get and Put similar to the Java behaviour
    /// </summary>
    class HashMap<K, V> : Dictionary<K, V>
    {
        /// <summary>
        /// Get a value by key
        /// </summary>
        /// <param name="key">The </param>
        /// <returns>The value or null if not found</returns>
        public V Get(K key)
        {
            if (this.TryGetValue(key, out V value))
            {
                return value;
            }
            return default(V);

        }

        /// <summary>
        /// Put a value in the map, overwriting any existing entry
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>The old value, or null</returns>
        public V Put(K key, V value)
        {
            V old_value = default(V);

            if (this.ContainsKey(key))
            {
                old_value = this[key];
                this[key] = value;
                
            }
            else
            {
                this.Add(key, value);
            }

            return old_value;
        }
    }
}
