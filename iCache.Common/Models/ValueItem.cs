using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    /// <summary>
    /// A key/value item
    /// </summary>
    public class ValueItem : KeyItem
    {
        [Required]
        [MinLength(1)]
        public string Value { get; set; }
    }
}
