using System;
using System.Threading.Tasks;
using TCache;

namespace iCache.API.Services
{
    public class KeyService : IDisposable
    {
        private TCacheService _cacheService;

        public async Task<bool> SetKey(string keyName, object value, int? expirationInSeconds)
        {
            return expirationInSeconds != null ?
                await _cacheService.SetObjectAsKeyValue(keyName, value, TimeSpan.FromSeconds((int)expirationInSeconds))
                :
                await _cacheService.SetObjectAsKeyValue(keyName, value);
        }

        public async Task<string> FetchKey(string keyName)
        {
            return await _cacheService.GetValueFromKey(keyName);
        }

        public async Task<bool> RemoveKey(string keyName)
        {
            return await _cacheService.RemoveKey(keyName);
        }

        public void Dispose()
        {
            ((IDisposable)_cacheService).Dispose();
        }
    }
}
