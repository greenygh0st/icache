using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCache.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCache.API.Controllers
{
    /// <summary>
    /// Provides basic up or down endpoints
    /// </summary>
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Meant to provide simple up or down endpoint
        /// </summary>
        /// <returns><see cref="JsonStatus"/></returns>
        [HttpGet]
        public JsonStatus Welcome()
        {
            return new JsonStatus { Message = "Welcome to the iCache service!" };
        }
    }
}