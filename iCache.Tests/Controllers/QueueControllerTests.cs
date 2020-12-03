using System;
using iCache.API.Controllers;
using Xunit;
using iCache.API.Services;
using System.Threading.Tasks;
using Moq;
using iCache.API.Services;
using iCache.API.Interfaces;
using System.Collections.Generic;
using iCache.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace iCache.Tests.Controllers
{
    public class QueueControllerTests
    {
        private Mock<IQueueService> _mockService;
        private QueueController _queueController;

        public QueueControllerTests()
        {
            // mock the queue service
            _mockService = new Mock<IQueueService>();

            // TODO: Setup the mocks
            _mockService.Setup(x => x.QueueExists("da-queue")).ReturnsAsync(true);
            _mockService.Setup(x => x.PullFromQueue("da-queue", true)).ReturnsAsync("a delete message");
            _mockService.Setup(x => x.PullFromQueue("da-queue", false)).ReturnsAsync("a pop message");
            _mockService.Setup(x => x.PushToQueue("da-queue", new List<string> { "test" })).ReturnsAsync(true);

            // create the controller
            _queueController = new QueueController(_mockService.Object);
            _queueController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { }
            };
        }

        [Fact]
        public async Task PushToQueue_BadModel()
        {
            // test setup
            var controller = new QueueController(_mockService.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { }
            };
            controller.ModelState.AddModelError("fakeError", "fakeError");

            // mock request
            JsonWithResponse response = await controller.PushToQueue(new QueueMessagesPost { QueueName = "da-queue" });

            Assert.Equal("Invalid queue request!", response.Message);
        }

        [Fact]
        public async Task PushToQueue()
        {
            QueueMessagesPost post = new QueueMessagesPost { QueueName = "da-queue", Messages = new List<string> { "test" } };

            JsonWithResponse response = await _queueController.PushToQueue(post);
            
            Assert.Equal($"Added {post.Messages.Count} to queue: {post.QueueName}", response.Message);
        }

        [Fact]
        public async Task PopFromQueue()
        {
            JsonWithResponse response = await _queueController.PopFromQueue("da-queue");

            Assert.Equal("success", response.Message);

            QueueMessage resActual = response.Response as QueueMessage;

            Assert.Equal("da-queue", resActual.QueueName);
            Assert.Equal("a pop message", resActual.Message);
        }

        [Fact]
        public async Task PopFromQueue_QueueDoesNotExist()
        {
            JsonWithResponse response = await _queueController.PopFromQueue("da-queue-2");
            Assert.Equal("Queue not found or is empty!", response.Message);
        }

        [Fact]
        public async Task DeleteFromQueue()
        {
            JsonWithResponse response = await _queueController.DeleteFromQueue("da-queue");

            Assert.Equal("deleted", response.Message);

            QueueMessage resActual = response.Response as QueueMessage;

            Assert.Equal("da-queue", resActual.QueueName);
            Assert.Equal("a delete message", resActual.Message);
        }

        [Fact]
        public async Task DeleteFromQueue_QueueDoesNotExis()
        {
            JsonWithResponse response = await _queueController.DeleteFromQueue("da-queue-2");
            Assert.Equal("Queue not found or is empty!", response.Message);
        }
    }
}
