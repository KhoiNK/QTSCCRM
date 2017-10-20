using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PostContactViewModel
    {
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public CustomB64ImageFileViewModel Avatar { get; set; }

        public Contact ToContactModel()
        {
            Contact _contact = new Contact
            {
                CustomerID = this.CustomerID,
                Name = this.Name,
                Position = this.Position,
                Email = this.Email,
                Phone = this.Phone
            };
            if (Avatar != null)
            {
                _contact.AvatarSrc = this.Avatar.Name;
            }
            return _contact;
        }
    }
}