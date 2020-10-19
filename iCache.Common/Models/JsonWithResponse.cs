using System;
namespace iCache.Common.Models
{
    public class JsonWithResponse : JsonStatus
    {
        public object Response { get; set; }
    }
}
