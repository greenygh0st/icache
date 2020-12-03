using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    public class KeySearch
    {
        /// <summary>
        /// The term you want to search with
        /// </summary>
        [Required]
        public string SearchTerm { get; set; }
        /// <summary>
        /// Include the values for the key in the response
        /// </summary>
        public bool IncludeValues { get; set; }
    }
}
