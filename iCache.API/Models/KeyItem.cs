using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.API.Models
{
    public class KeyItem
    {
        [Required]
        public string Key { get; set; }
    }
}
