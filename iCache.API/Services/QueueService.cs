using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCache;

namespace iCache.API.Services
{
    public class QueueService : IDisposable
    {
        private TCacheService _cacheService;

        public QueueService()
        {
            _cacheService = new TCacheService(Configuration.RedisConnection);
        }

        public async Task<bool> PushToQueue(string queueName, List<string> values)
        {
            return await _cacheService.PushToQueue(queueName, values);
        }

        public async Task<string> PullFromQueue(string queueName, bool delete = true)
        {
            TCachePopMode popMode = delete ? TCachePopMode.Delete : TCachePopMode.Get;
            return await _cacheService.PopFromQueue(queueName, popMode);
        }

        public async Task<bool> QueueExists(string queueName)
        {
            return await _cacheService.QueueExists(queueName);
        }

        public async Task<bool> RemoveQueue(string queueName)
        {
            return await _cacheService.RemoveQueue(queueName);
        }

        public void Dispose()
        {
            ((IDisposable)_cacheService).Dispose();
        }
    }
}
