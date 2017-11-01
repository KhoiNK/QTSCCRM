using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class StaffDetailViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string AvatarUrl { get; set; }

        public StaffDetailViewModel(Staff dto)
        {
            this.ID = dto.ID;
            this.Name = dto.Name;
            this.Phone = dto.Phone;
            this.Email = dto.Email;
            this.RoleName = dto.Role.Name;
            this.RoleID = dto.RoleID;
            this.AvatarUrl = dto.AvatarSrc;
        }
    }
}