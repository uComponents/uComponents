using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace uComponents.Mapping
{
    /// <summary>
    /// Cache provider for the <c>System.Web.Caching.Cache</c>.
    /// </summary>
    internal class DefaultCacheProvider : ICacheProvider
    {
        private const int _slidingExpirationSeconds = 10 * 60; // ten minutes

        // Cannot insert null into cache, so use a static representation.
        private const string _nullValue = "840CCFEA-6D44-400F-BC3F-806C78E388B9";

        private readonly Cache _cache;
        private readonly Guid _keyPrefix;

        public DefaultCacheProvider(Cache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            _cache = cache;
            _keyPrefix = Guid.NewGuid();
        }

        public void Insert(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", "key");
            }
            else if (value == null)
            {
                value = _nullValue;
            }

            var qualifiedKey = GetQualifiedKey(key);

            _cache.Insert(qualifiedKey, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationSeconds));
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", "key");
            }

            var qualifiedKey = GetQualifiedKey(key);
            var value = _cache[qualifiedKey];

            return value == _nullValue ? null : value;
        }

        public bool ContainsKey(string key)
        {
            return _cache[key] != _nullValue;
        }

        /// <summary>
        /// Removes all cache items added by this cache provider.
        /// </summary>
        public void Clear()
        {
            var enumerator = _cache.GetEnumerator();
            var keysToRemove = new List<string>();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key as string;

                if (key != null && key.StartsWith(_keyPrefix.ToString()))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Constructs a cache key specific to this cache provider.
        /// </summary>
        /// <param name="key">The key to qualify.</param>
        private string GetQualifiedKey(string key)
        {
            return string.Format("{0}_{1}", _keyPrefix.ToString(), key);
        }
    }

    /// <summary>
    /// Convenience methods for the <see cref="ICacheProvider"/>, as required
    /// by <see cref="INodeMappingEngine"/>.
    /// </summary>
    public static class CacheProviderExtensions
    {
        private const string _propertyValueFormat = "{0}_{1}";
        private const string _aliasFormat = "{0}_DocumentTypeAlias";

        public static void InsertPropertyValue(this ICacheProvider cache, int id, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("The property name cannot be null or empty", "propertyName");
            }

            var key = string.Format(_propertyValueFormat, id, propertyName);

            cache.Insert(key, value);
        }

        public static void InsertAlias(this ICacheProvider cache, int id, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentException("The alias cannot be null or empty", "alias");
            }

            var key = string.Format(_aliasFormat, id);

            cache.Insert(key, alias);
        }

        public static object GetPropertyValue(this ICacheProvider cache, int id, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("The property name cannot be null or empty", "propertyName");
            }

            var key = string.Format(_propertyValueFormat, id, propertyName);

            return cache.Get(key);
        }

        public static string GetAlias(this ICacheProvider cache, int id)
        {
            var key = string.Format(_aliasFormat, id);

            return cache.Get(key) as string;
        }

        public static bool ContainsPropertyValue(this ICacheProvider cache, int id, string propertyName)
        {
            return cache.ContainsKey(string.Format(_propertyValueFormat, id, propertyName));
        }

        public static bool ContainsAlias(this ICacheProvider cache, int id)
        {
            return cache.ContainsKey(string.Format(_aliasFormat, id));
        }
    }
}
