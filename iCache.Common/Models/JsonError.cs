using System;
using System.Collections.Generic;

namespace iCache.Common.Models
{
    public class JsonError : JsonWithResponse
    {
        public List<string> Errors { get; set; }
    }
}
