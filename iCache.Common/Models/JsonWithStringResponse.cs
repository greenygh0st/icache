using System;
namespace iCache.Common.Models
{
    public class JsonWithStringResponse : JsonStatus
    {
        public string Response { get; set; }
    }
}
