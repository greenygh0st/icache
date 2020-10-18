using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class KeyItem
    {
        [Required]
        public string Key { get; set; }
    }
}
