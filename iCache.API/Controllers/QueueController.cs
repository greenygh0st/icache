using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCache.Common.Models;
using iCache.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCache.API.Controllers
{
    [Route("api/queue")]
    [ApiController]
    [Authorize]
    public class QueueController : ControllerBase
    {
        [HttpGet("{queueName}")]
        public async Task<IActionResult> PopFromQueue(string queueName)
        {
            using (QueueService queueService = new QueueService())
            {
                if (await queueService.QueueExists(queueName))
                {
                    string message = await queueService.PullFromQueue(queueName, false);

                    return Ok(new QueueMessage {
                        QueueName = queueName,
                        Message = message
                    });
                } else
                {
                    return NotFound(new JsonStatus { Message = "Queue not found or is empty!" });
                }
            }
        }

        [HttpDelete("{queueName}")]
        public async Task<IActionResult> DeleteFromQueue(string queueName)
        {
            using (QueueService queueService = new QueueService())
            {
                if (await queueService.QueueExists(queueName))
                {
                    string message = await queueService.PullFromQueue(queueName, true);

                    return Ok(new QueueMessage
                    {
                        QueueName = queueName,
                        Message = message
                    });
                }
                else
                {
                    return NotFound(new JsonStatus { Message = "Queue not found or is empty!" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PushToQueue([FromBody] QueueMessages queueMessages)
        {
            if (ModelState.IsValid)
            {
                using (QueueService queueService = new QueueService())
                {
                    await queueService.PushToQueue(queueMessages.QueueName, queueMessages.Messages);
                    
                    return Ok(new JsonStatus { Message = $"Added {queueMessages.Messages.Count} to queue: {queueMessages.QueueName}" });
                    
                }
            } else
            {
                return BadRequest(new JsonError
                {
                    Message = "Invalid queue request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                });
            }
        }
    }
}