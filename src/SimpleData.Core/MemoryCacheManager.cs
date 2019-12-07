using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleData.Core
{
    public class MemoryCache
    {
        public MemoryCache(object cache)
        {
            this.Cache = cache;
            this.CacheTime = DateTime.Now;
        }

        public object Cache { get; }
        public DateTime CacheTime { get; }
    }

    public class MemoryCacheManager
    {
        public MemoryCacheManager(TimeSpan? expireTimeSpan = null)
        {
            if (expireTimeSpan != null)
                this._expireTimeSpan = expireTimeSpan.Value;
        }

        private TimeSpan _expireTimeSpan = TimeSpan.FromMinutes(60);

        private IDictionary<string, MemoryCache> _caches = new Dictionary<string, MemoryCache>();

        private object _lock = new object();

        public void AddCache(string name, object cache)
        {
            if (_caches.Keys.Contains(name))
                throw new InvalidOperationException("缓存中存在重复键");

            lock(_lock)
                _caches.Add(name, new MemoryCache(cache));
        }

        public object GetCache(string name)
        {
            if (!_caches.Keys.Contains(name))
                throw new KeyNotFoundException("缓存未找到指定键");

            return _caches[name];
        }

        public void RemoveCache(string name)
        {
            if (!_caches.Keys.Contains(name))
                throw new KeyNotFoundException("缓存未找到指定键");

            lock (_lock)
                _caches.Remove(name);
        }

        public void UpdateCache(string name, object obj)
        {
            if (!_caches.Keys.Contains(name))
                throw new KeyNotFoundException("缓存未找到指定键");

            lock (_lock)
                _caches[name] = new MemoryCache(obj);
        }

        public bool CheckIfExpired(string name)
        {
            if (!_caches.Keys.Contains(name))
                throw new KeyNotFoundException("缓存未找到指定键");

            if (DateTime.Now - _caches[name].CacheTime > this._expireTimeSpan)
                return true;

            return false;
        }

        public bool CheckIfCahceExists(string name)
        {
            if (!_caches.Keys.Contains(name))
                return false;
            return true;
        }
    }
}
