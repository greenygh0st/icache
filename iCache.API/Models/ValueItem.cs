using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.API.Models
{
    public class ValueItem : KeyItem
    {
        [Required]
        public string Value { get; set; }
    }
}
