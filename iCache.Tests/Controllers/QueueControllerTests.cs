using System;
using iCache.API.Controllers;
using Xunit;
using iCache.API.Services;
using System.Threading.Tasks;

namespace iCache.Tests.Controllers
{
    public class QueueControllerTests
    {
        public QueueControllerTests()
        {
            using (QueueService queueService = new QueueService())
            {

            }
        }

        [Fact]
        public async Task EmptyQueue()
        {
            //var controller = new QueueController();

            //var result = await controller.PopFromQueue("test-queue-1");

            //Assert.Equal("Queue not found or is empty!", result.Message);
        }
    }
}
