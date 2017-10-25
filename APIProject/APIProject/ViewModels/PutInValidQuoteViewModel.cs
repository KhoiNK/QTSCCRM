using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutInValidQuoteViewModel
    {

        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public string Notes { get; set; }
    }
}