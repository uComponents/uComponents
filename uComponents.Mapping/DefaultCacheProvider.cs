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
        private const string _propertyValueFormat = "{0}_{1}";
        private const string _aliasFormat = "{0}_DocumentTypeAlias";

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
                throw new ArgumentNullException("value");
            }

            var qualifiedKey = GetQualifiedKey(key);

            _cache.Insert(qualifiedKey, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationSeconds));
        }

        public void InsertPropertyValue(int id, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("The property name cannot be null or empty", "propertyName");
            }
            else if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var key = string.Format(_propertyValueFormat, id, propertyName);

            Insert(key, value);
        }

        public void InsertAlias(int id, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentException("The alias cannot be null or empty", "alias");
            }

            var key = string.Format(_aliasFormat, id);

            Insert(key, alias);
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", "key");
            }

            var qualifiedKey = GetQualifiedKey(key);

            return _cache[qualifiedKey];
        }

        public object GetPropertyValue(int id, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("The property name cannot be null or empty", "propertyName");
            }

            var key = string.Format(_propertyValueFormat, id, propertyName);

            return Get(key);
        }

        public string GetAlias(int id)
        {
            var key = string.Format(_aliasFormat, id);

            return Get(key) as string;
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
}
