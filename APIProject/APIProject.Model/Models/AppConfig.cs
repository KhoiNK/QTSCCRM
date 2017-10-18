using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Model.Models
{
    [Table("AppConfig")]
    public partial class AppConfig
    {
        [Key]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
