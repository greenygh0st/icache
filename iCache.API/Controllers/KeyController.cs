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
        /// Get the value for the specified key from your user context
        /// </summary>
        /// <param name="key">The key for which you want to retrieve a value</param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<JsonWithResponse> GetKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                using (KeyService keyService = new KeyService())
                {
                    if (await keyService.KeyExists($"{User.Identity.Name}:{key}"))
                    {
                        return new JsonWithResponse
                        {
                            Message = "success",
                            Response = new ValueItem
                            {
                                Key = key,
                                Value = await keyService.FetchKey($"{User.Identity.Name}:{key}")
                            }
                        };
                    } else
                    {
                        Response.StatusCode = 404;
                        return new JsonWithResponse { Message = "Key not found!" };
                    }
                }
            } else
            {
                Response.StatusCode = 400;
                return new JsonWithResponse { Message = "Key value not supplied!" };
            }
        }

        /// <summary>
        /// Delete a key from your user context
        /// </summary>
        /// <param name="key">The key which you want to remove</param>
        /// <returns><see cref="JsonStatus"/> indicated if the operation was successful</returns>
        [HttpDelete("{key}")]
        public async Task<JsonStatus> DeleteKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                using (KeyService keyService = new KeyService())
                {
                    if (await keyService.KeyExists($"{User.Identity.Name}:{key}"))
                    {
                        await keyService.RemoveKey($"{User.Identity.Name}:{key}");

                        return new JsonStatus
                        {
                            Message = "Key deleted!"
                        };
                    }
                    else
                    {
                        Response.StatusCode = 404;
                        return new JsonStatus { Message = "Key not found!" };
                    }
                }
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonStatus { Message = "Key not found!" };
            }
        }

        /// <summary>
        /// Add a key to your user context
        /// </summary>
        /// <param name="value"><see cref="CreateValueItem"/>Which contains the desired key and value</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonWithResponse> CreateKey([FromBody] CreateValueItem value)
        {
            if (ModelState.IsValid)
            {
                using (KeyService keyService = new KeyService())
                {
                    bool set = (value.Expiration == null) ?
                        await keyService.SetKey($"{User.Identity.Name}:{value.Key}", value.Value)
                        :
                        await keyService.SetKey($"{User.Identity.Name}:{value.Key}", value.Value, (int)value.Expiration);

                    Response.StatusCode = 201;
                    return new JsonWithResponse {
                        Message = "created",
                        Response = new ValueItem {
                            Key = value.Key,
                            Value = value.Value
                        }
                    };
                }
            } else
            {
                Response.StatusCode = 400;
                return new JsonError {
                    Message = "Invalid key request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                };
            }
        }
    }
}