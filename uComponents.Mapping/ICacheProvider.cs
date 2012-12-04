using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Mapping
{
    /// <summary>
    /// A cache provider which satisfies the caching requirements of uMapper.
    /// 
    /// TODO: refactor with specific methods for getting node properties
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
        /// Inserts a mapped property into the cache.
        /// </summary>
        /// <param name="id">The ID of the node which was mapped from.</param>
        /// <param name="propertyName">The model property's unqualified name, e.g. "Weight".</param>
        /// <param name="value">The mapped property. Can be null.</param>
        void InsertPropertyValue(int id, string propertyName, object value);

        /// <summary>
        /// Inserts a document type alias for a node ID.
        /// </summary>
        /// <param name="id">The ID of the node.</param>
        /// <param name="alias">The document type alias.</param>
        void InsertAlias(int id, string alias);

        /// <summary>
        /// Gets an object from the cache.
        /// </summary>
        /// <param name="key">The string which represents the object.</param>
        /// <returns>
        /// null if there is no object with the specified <paramref name="key"/>
        /// </returns>
        object Get(string key);

        /// <summary>
        /// Gets a mapped property from the cache.
        /// </summary>
        /// <param name="id">The ID of the node which was mapped from.</param>
        /// <param name="propertyName">The model property's unqualified name, e.g. "Weight".</param>
        object GetPropertyValue(int id, string propertyName);

        /// <summary>
        /// Gets the document type alias for a node ID.
        /// </summary>
        /// <param name="id">The ID of the node to get the alias for.</param>
        /// <returns><c>null</c> if not inserted.</returns>
        string GetAlias(int id);

        /// <summary>
        /// Checks if the cache contains a value for the key.
        /// </summary>
        bool ContainsKey(string key);

        /// <summary>
        /// Checks if the cache contains a property value for the property.
        /// </summary>
        bool ContainsPropertyValue(int id, string propertyName);

        /// <summary>
        /// Checks if the cache contains an alias for the node ID.
        /// </summary>
        bool ContainsAlias(int id);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();
    }
}
