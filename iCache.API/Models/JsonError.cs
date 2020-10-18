using System;
using System.Collections.Generic;

namespace iCache.API.Models
{
    public class JsonError : JsonStatus
    {
        public List<string> Errors { get; set; }
    }
}
