using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class CreateValueItem : ValueItem
    {
        /// <summary>
        /// Expiration in seconds
        /// </summary>
        public int? Expiration { get; set; }
    }
}
