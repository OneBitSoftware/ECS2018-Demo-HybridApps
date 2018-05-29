using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication
{
    public class SessionCacheService : TokenCache
    {
        private static readonly object _objectLock = new object();
        string _userId = string.Empty;
        string _cacheId = string.Empty;
        IMemoryCache _memoryCache;
        TokenCache _cache = new TokenCache();

        public SessionCacheService(string userId, IMemoryCache memoryCache)
        {
            // not object, we want the SUB
            _userId = userId;
            _cacheId = userId + "_TokenCache";
            _memoryCache = memoryCache;

            Load();
        }

        public TokenCache GetCacheInstance()
        {
            _cache.BeforeAccess = BeforeAccessNotification;
            _cache.AfterAccess = AfterAccessNotification;
            Load();

            return _cache;
        }

        public void SaveUserStateValue(string state)
        {
            lock (_objectLock)
            {
                _memoryCache.Set(_cacheId + "_state", Encoding.ASCII.GetBytes(state));
            }
        }

        public string ReadUserStateValue()
        {
            string state = string.Empty;
            lock (_objectLock)
            {
                state = Encoding.ASCII.GetString(_memoryCache.Get(_cacheId + "_state") as byte[]);
            }

            return state;
        }

        public void Load()
        {
            lock (_objectLock)
            {
                _cache.Deserialize(_memoryCache.Get(_cacheId) as byte[]);
            }
        }

        public void Persist()
        {
            lock (_objectLock)
            {
                // reflect changes in the persistent store
                _memoryCache.Set(_cacheId, _cache.Serialize());
                // once the write operation took place, restore the HasStateChanged bit to false
                _cache.HasStateChanged = false;
            }
        }

        // Empties the persistent store.
        public override void Clear()
        {
            // NOTE: we added the override keyword and called the 
            // base.Clear() to avoid compilation warnings
            _cache = null;
            _memoryCache.Remove(_cacheId);
            base.Clear();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (_cache.HasStateChanged)
            {
                Persist();
            }
        }
    }
}
