using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCache.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCache.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public JsonStatus Welcome()
        {
            return new JsonStatus { Message = "Welcome to the iCache service!" };
        }
    }
}