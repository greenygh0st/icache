using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iCache.API.Interfaces;
using TCache;

namespace iCache.API.Services
{
    public class KeyService : IDisposable, IKeyService
    {
        private TCacheService _cacheService;

        public KeyService()
        {
            _cacheService = new TCacheService(Configuration.RedisConnection);
        }

        public async Task<bool> SetKey(string keyName, object value)
        {
            return await _cacheService.SetObjectAsKeyValue(keyName, value);
        }

        public async Task<bool> SetKey(string keyName, object value, int expirationInSeconds)
        {
            return await _cacheService.SetObjectAsKeyValue(keyName, value, TimeSpan.FromSeconds(expirationInSeconds));
        }

        public async Task<string> FetchKey(string keyName)
        {
            return await _cacheService.GetValueFromKey(keyName);
        }

        public async Task<bool> RemoveKey(string keyName)
        {
            return await _cacheService.RemoveKey(keyName);
        }

        public async Task<bool> KeyExists(string keyName)
        {
            return await _cacheService.KeyExists(keyName);
        }

        public async Task<List<string>> SearchKeys(string searchTerm)
        {
            return await _cacheService.SearchKeys(searchTerm);
        }

        public async Task<Dictionary<string, string>> SearchKeysGetValues(string searchTerm)
        {
            return await _cacheService.SearchKeyValues(searchTerm);
        }

        public void Dispose()
        {
            ((IDisposable)_cacheService).Dispose();
        }
    }
}
