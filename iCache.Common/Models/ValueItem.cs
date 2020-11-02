﻿using System;
using System.ComponentModel.DataAnnotations;

namespace iCache.Common.Models
{
    /// <summary>
    /// A key/value item
    /// </summary>
    public class ValueItem : KeyItem
    {
        [Required]
        public string Value { get; set; }
    }
}
