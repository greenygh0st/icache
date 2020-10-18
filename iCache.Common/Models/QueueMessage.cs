using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class QueueMessage
    {
        [Required]
        public string QueueName { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
