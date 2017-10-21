using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutCancelActivityViewModel
    {
        [Required]
        public int ID { get; set; }
    }
}