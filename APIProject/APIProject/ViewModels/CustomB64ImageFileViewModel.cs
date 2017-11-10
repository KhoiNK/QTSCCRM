using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class CustomB64ImageFileViewModel
    {
        [Required]
        //[RegularExpression(@"[^\s]+(\.(?i)(jpg|png|gif|bmp))$", ErrorMessage = "Chỉ đc jpg,png,gif,bmp")]
        public string Name { get; set; }
        [Required]
        public string Base64Content { get; set; }
    }
}