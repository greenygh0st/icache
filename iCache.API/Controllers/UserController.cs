using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iCache.Common.Models;
using iCache.API.Services;

namespace iCache.API.Controllers
{
    /// <summary>
    /// Add, remove and update user password. Note: this controller is only accessible with the pre-defined admin credentials.
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Create a user. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="createUser"><see cref="CreateUser"/></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser)
        {
            if (ModelState.IsValid)
            {
                using (UserService userService = new UserService())
                {
                    User user = await userService.CreateUser(createUser);

                    return Created("", new JsonWithResponse { Message = "created", Response = user });
                }
            } else
            {
                return BadRequest(new JsonError
                {
                    Message = "Invalid user request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                });
            }
        }

        /// <summary>
        /// Remove a current user. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="userId">The id of the user that you want to remove</param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            using (UserService userService = new UserService())
            {
                if (await userService.UserExists(new User { _Id = userId }))
                {
                    await userService.RemoveUser(new User { _Id = userId });

                    return Ok(new JsonStatus { Message = "User removed!" });
                } else
                {
                    return NotFound(new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } });
                }
            }
        }

        /// <summary>
        /// Regenerate a user password. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="userId">The password of the user that you want to update</param>
        /// <returns></returns>
        [HttpPost("{userId}/password")]
        public async Task<IActionResult> RegenUserPassword(Guid userId)
        {
            using (UserService userService = new UserService())
            {
                if (await userService.UserExists(new User { _Id = userId }))
                {
                    User userWithNewPassword = await userService.RegeneratePassword(new User { _Id = userId });

                    return Ok(new JsonWithResponse { Message = "success", Response = userWithNewPassword });
                }
                else
                {
                    return NotFound(new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } });
                }
            }
        }
    }
}