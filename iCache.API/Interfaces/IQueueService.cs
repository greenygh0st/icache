using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iCache.API.Interfaces
{
    public interface IQueueService
    {
        Task<bool> PushToQueue(string queueName, List<string> values);
        Task<string> PullFromQueue(string queueName, bool delete = true);
        Task<bool> QueueExists(string queueName);
        Task<bool> RemoveQueue(string queueName);
    }
}
