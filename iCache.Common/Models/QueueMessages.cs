using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace iCache.Common.Models
{
    public class QueueMessages
    {
        [Required]
        [JsonProperty("queueName")]
        public string QueueName { get; set; }
        [Required]
        [MinLength(1)]
        [JsonProperty("messages")]
        public List<object> Messages { get; set; }
    }
}
