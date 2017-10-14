using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomBase64FileViewModel
    {
        public string Name { get; set; }
        [Required]
        public string Base64Content { get; set; }
    }
}