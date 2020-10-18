using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class ValueItem : KeyItem
    {
        [Required]
        public string Value { get; set; }
    }
}
