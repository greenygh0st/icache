using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCache.Common.Models;
using iCache.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iCache.API.Interfaces;

namespace iCache.API.Controllers
{
    /// <summary>
    /// Allows you to add messages to queues. Queues are not user context specific.
    /// </summary>
    [Route("api/queue")]
    [ApiController]
    [Authorize]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="queueService">Queue service</param>
        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        /// <summary>
        /// Pop a message from a queue. Using this method the popped message remains in the queue until delete is called. Note: Queues are not user context specific.
        /// </summary>
        /// <param name="queueName">The queue you want to pop from</param>
        /// <returns>JsonResponse with QueueMessage</returns>
        [HttpGet("{queueName}")]
        public async Task<JsonWithResponse> PopFromQueue(string queueName)
        {
            if (await _queueService.QueueExists(queueName))
            {
                string message = await _queueService.PullFromQueue(queueName, false);

                return new JsonWithResponse
                {
                    Message = "success",
                    Response = new QueueMessage
                    {
                        QueueName = queueName,
                        Message = message
                    }
                };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonWithResponse { Message = "Queue not found or is empty!" };
            }
        }

        /// <summary>
        /// Pop and delete a message from the queue. Note: Queues are not user context specific.
        /// </summary>
        /// <param name="queueName">The queue you want to delete from</param>
        /// <returns>JsonResponse with QueueMessage</returns>
        [HttpDelete("{queueName}")]
        public async Task<JsonWithResponse> DeleteFromQueue(string queueName)
        {
            if (await _queueService.QueueExists(queueName))
            {
                string message = await _queueService.PullFromQueue(queueName, true);

                return new JsonWithResponse
                {
                    Message = "deleted",
                    Response = new QueueMessage
                    {
                        QueueName = queueName,
                        Message = message
                    }
                };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonWithResponse { Message = "Queue not found or is empty!" };
            }
        }

        /// <summary>
        /// Push a set of messages to a queue. Note: Queues are not user context specific, make sure you are pushing the correct message type.
        /// </summary>
        /// <param name="queueMessages"><see cref="QueueMessages"/>. A set of messages you want to push to a queue</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonWithResponse> PushToQueue([FromBody] QueueMessagesPost queueMessages)
        {
            if (ModelState.IsValid)
            {
                await _queueService.PushToQueue(queueMessages.QueueName, queueMessages.Messages);

                return new JsonWithResponse { Message = $"Added {queueMessages.Messages.Count} to queue: {queueMessages.QueueName}" };
            } else
            {
                Response.StatusCode = 400;
                return new JsonError
                {
                    Message = "Invalid queue request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                };
            }
        }
    }
}