using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace uComponents.Mapping
{
    internal class DefaultCacheProvider : ICacheProvider
    {
        private const int _defaultCacheLengthInSeconds = 10 * 60; 
        private readonly Cache _cache;

        public DefaultCacheProvider(Cache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            _cache = cache;
        }

        public void Insert(string key, object value)
        {
            _cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_defaultCacheLengthInSeconds));
        }

        public object Get(string key)
        {
            return _cache[key];
        }
    }
}
