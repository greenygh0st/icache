using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iCache.API.Interfaces
{
    public interface IKeyService
    {
        Task<bool> SetKey(string keyName, object value);
        Task<bool> SetKey(string keyName, object value, int expirationInSeconds);
        Task<string> FetchKey(string keyName);
        Task<bool> RemoveKey(string keyName);
        Task<bool> KeyExists(string keyName);
        Task<List<string>> SearchKeys(string searchTerm);
        Task<Dictionary<string, string>> SearchKeysGetValues(string searchTerm);
    }
}
