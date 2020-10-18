using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class CreateUser
    {
        [Required]
        [MinLength(6)]
        public string DisplayName { get; set; }
    }
}
