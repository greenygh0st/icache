using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iCache.Common.Models;
using iCache.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace iCache.API.Controllers
{
    /// <summary>
    /// The key controller allows you to set and retrieve user/key values within your user context
    /// </summary>
    [Route("api/key")]
    [ApiController]
    [Authorize]
    public class KeyController : ControllerBase
    {
        /// <summary>
        /// Get the value for the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<IActionResult> GetKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                using (KeyService keyService = new KeyService())
                {
                    if (await keyService.KeyExists($"{User.Identity.Name}:{key}"))
                    {
                        return Ok(new JsonWithResponse
                        {
                            Message = "success",
                            Response = new ValueItem
                            {
                                Key = key,
                                Value = await keyService.FetchKey($"{User.Identity.Name}:{key}")
                            }
                        });
                    } else
                    {
                        return NotFound(new JsonStatus { Message = "Key not found!" });
                    }
                }
            } else
            {
                return BadRequest(new JsonStatus { Message = "Key value not supplied!" });
            }
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                using (KeyService keyService = new KeyService())
                {
                    if (await keyService.KeyExists($"{User.Identity.Name}:{key}"))
                    {
                        await keyService.RemoveKey($"{User.Identity.Name}:{key}");

                        return Ok(new JsonStatus
                        {
                            Message = "Key deleted!"
                        });
                    }
                    else
                    {
                        return NotFound(new JsonStatus { Message = "Key not found!" });
                    }
                }
            }
            else
            {
                return NotFound(new JsonStatus { Message = "Key not found!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateKey([FromBody] CreateValueItem value)
        {
            if (ModelState.IsValid)
            {
                using (KeyService keyService = new KeyService())
                {
                    bool set = (value.Expiration == null) ?
                        await keyService.SetKey($"{User.Identity.Name}:{value.Key}", value.Value)
                        :
                        await keyService.SetKey($"{User.Identity.Name}:{value.Key}", value.Value, (int)value.Expiration);

                    return Created($"/key/{value.Key}", new JsonWithResponse {
                        Message = "created",
                        Response = new ValueItem {
                            Key = value.Key,
                            Value = value.Value
                        }
                    });
                }
            } else
            {
                return BadRequest(new JsonError {
                    Message = "Invalid key request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                });
            }
        }
    }
}