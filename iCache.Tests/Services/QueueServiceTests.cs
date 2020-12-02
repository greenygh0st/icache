using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iCache.API.Services;
using Xunit;

namespace iCache.Tests.Services
{
    public class QueueServiceTests
    {
        [Fact]
        public async Task PushToQueue()
        {
            using (QueueService queueService = new QueueService())
            {
                const string queueName = "testQueue";

                await queueService.QueueExists(queueName);

                await queueService.PushToQueue(queueName, new List<string> { "123" });

                Assert.True(await queueService.QueueExists(queueName));

                await queueService.RemoveQueue(queueName);

                Assert.False(await queueService.QueueExists(queueName));
            }
        }

        [Fact]
        public async Task PullFromQueue()
        {
            using (QueueService queueService = new QueueService())
            {
                const string queueName = "testQueue1";

                Assert.False(await queueService.QueueExists(queueName));

                await queueService.PushToQueue(queueName, new List<string> { "123" });

                Assert.True(await queueService.QueueExists(queueName));

                Assert.NotNull(await queueService.PullFromQueue(queueName));

                Assert.False(await queueService.QueueExists(queueName));
            }
        }

        [Fact]
        public async Task PullFromQueueDelete()
        {
            using (QueueService queueService = new QueueService())
            {
                const string queueName = "testQueue2";

                await queueService.QueueExists(queueName);

                await queueService.PushToQueue(queueName, new List<string> { "123" });

                Assert.NotNull(await queueService.PullFromQueue(queueName, false));

                Assert.True(await queueService.QueueExists(queueName));

                await queueService.RemoveQueue(queueName);

                Assert.Null(await queueService.PullFromQueue(queueName));
            }
        }
    }
}
