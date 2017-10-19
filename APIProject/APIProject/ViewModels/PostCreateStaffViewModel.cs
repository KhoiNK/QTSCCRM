using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostCreateStaffViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int RoleID { get; set; }

        public Staff ToStaffModel()
        {
            return new Staff
            {
                Name=this.Name,
                Username=this.Username,
                Phone=this.Phone,
                Email=this.Email,
                RoleID=this.RoleID
            };
        }
    }
}