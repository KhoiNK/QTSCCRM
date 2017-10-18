﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomBase64FileViewModel
    {
        [Required]
        [RegularExpression(@"[^\s]+(\.(?i)(jpg|png|gif|bmp))$", ErrorMessage = "Sai định dạng")]
        public string Name { get; set; }
        [Required]
        public string Base64Content { get; set; }
    }
}