using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class QueueMessages
    {
        [Required]
        public string QueueName { get; set; }
        [Required]
        [MinLength(1)]
        public List<string> Messages { get; set; }
    }
}
