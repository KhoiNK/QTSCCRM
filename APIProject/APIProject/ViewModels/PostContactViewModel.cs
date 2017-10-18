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
            Contact _contact = new Contact();
            _contact.CustomerID = this.CustomerID;
            _contact.Name = this.Name;
            _contact.Position = this.Position;
            _contact.Email = this.Email;
            _contact.Phone = this.Phone;
            if (Avatar != null)
            {
                _contact.AvatarSrc = this.Avatar.Name;
            }
            return _contact;
        }
    }
}