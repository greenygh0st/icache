using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    /// <summary>
    /// Base key item object
    /// </summary>
    public class KeyItem
    {
        [Required]
        public string Key { get; set; }
    }
}
