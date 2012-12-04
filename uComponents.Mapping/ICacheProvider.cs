using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Mapping
{
    /// <summary>
    /// A cache provider which satisfies the caching requirements of uMapper.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Inserts an object into the cache, replacing an existing object
        /// if one already exists with the same <paramref name="key"/>.
        /// </summary>
        /// <param name="value">The value to set. Can be null.</param>
        void Insert(string key, object value);

        /// <summary>
        /// Gets an object from the cache.
        /// </summary>
        /// <param name="key">The string which represents the object.</param>
        /// <returns>
        /// null if there is no object with the specified <paramref name="key"/>
        /// </returns>
        object Get(string key);

        /// <summary>
        /// Checks if the cache contains a value for the key.
        /// </summary>
        bool ContainsKey(string key);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();
    }
}
