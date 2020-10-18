using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCache.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCache.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class UserController : ControllerBase
    {
    }
}