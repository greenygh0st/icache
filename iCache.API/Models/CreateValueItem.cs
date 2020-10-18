using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.API.Models
{
    public class CreateValueItem : ValueItem
    {
        /// <summary>
        /// Expiration in seconds
        /// </summary>
        public int? Expiration { get; set; }
    }
}
