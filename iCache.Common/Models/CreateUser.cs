using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    /// <summary>
    /// Used to create a user
    /// </summary>
    public class CreateUser
    {
        [Required]
        [MinLength(6)]
        public string DisplayName { get; set; }
    }
}
