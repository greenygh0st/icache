using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iCache.Common.Models;
using iCache.API.Services;
using iCache.API.Interfaces;

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
        IUserService _userService;

        /// <summary>
        /// Base contructor
        /// </summary>
        /// <param name="userService">Service injection</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create a user. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="createUser"><see cref="CreateUser"/></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonWithResponse> CreateUser([FromBody] CreateUser createUser)
        {
            if (ModelState.IsValid)
            {
                User user = await _userService.CreateUser(createUser);

                return new JsonWithResponse { Message = "created", Response = user };
            } else
            {
                Response.StatusCode = 400;
                return new JsonError
                {
                    Message = "Invalid user request!",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToList()
                };
            }
        }

        /// <summary>
        /// Remove a current user. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="userId">The id of the user that you want to remove</param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task<JsonWithResponse> RemoveUser(Guid userId)
        {
            User _user = new User { _Id = userId };

            if (await _userService.UserExists(_user))
            {
                await _userService.RemoveUser(_user);

                return new JsonWithResponse { Message = "User removed!" };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } };
            }
        }

        /// <summary>
        /// Regenerate a user password. Note: Only accessible with the pre-defined admin credentials.
        /// </summary>
        /// <param name="userId">The password of the user that you want to update</param>
        /// <returns></returns>
        [HttpPost("{userId}/password")]
        public async Task<JsonWithResponse> RegenUserPassword(Guid userId)
        {
            if (await _userService.UserExists(new User { _Id = userId }))
            {
                User userWithNewPassword = await _userService.RegeneratePassword(new User { _Id = userId });

                return new JsonWithResponse { Message = "success", Response = userWithNewPassword };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } };
            }
        }

        /// <summary>
        /// Lock a user account.
        /// </summary>
        /// <param name="userId">The user id that you want to unlock</param>
        /// <returns></returns>
        [HttpGet("{userId}/lock")]
        public async Task<JsonStatus> LockUserAccount(Guid userId)
        {
            if (await _userService.UserExists(new User { _Id = userId }))
            {
                bool locked = await _userService.LockUser(new User { _Id = userId });

                return new JsonStatus { Message = $"User account {userId} locked!" };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } };
            }
        }

        /// <summary>
        /// Unlock a user account which has been locked.
        /// </summary>
        /// <param name="userId">The user id that you want to unlock</param>
        /// <returns></returns>
        [HttpGet("{userId}/unlock")]
        public async Task<JsonStatus> UnlockUserAccount(Guid userId)
        {
            if (await _userService.UserExists(new User { _Id = userId }))
            {
                bool unlocked = await _userService.UnlockUser(new User { _Id = userId });

                return new JsonStatus { Message = $"User account {userId} unlocked!" };
            }
            else
            {
                Response.StatusCode = 404;
                return new JsonError { Message = "Not found", Errors = new List<string> { $"{userId} was not found!" } };
            }
        }
    }
}