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
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser)
        {
            if (ModelState.IsValid)
            {

            } else
            {

            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {

        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> RegenUserPassword(Guid userId)
        {

        }
    }
}