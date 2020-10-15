using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCache;

namespace iCache.API.Services
{
    public class QueueService : IDisposable
    {
        private TCacheService _cacheService;

        public async Task<bool> PushToQueue(string queueName, List<string> values)
        {
            return await _cacheService.PushToQueue(queueName, values);
        }

        public async Task<string> PullFromQueue(string queueName, bool dontDelete = true)
        {
            TCachePopMode popMode = (TCachePopMode)Convert.ToInt32(dontDelete);
            return await _cacheService.PopFromQueue(queueName, popMode);
        }

        public void Dispose()
        {
            ((IDisposable)_cacheService).Dispose();
        }
    }
}
