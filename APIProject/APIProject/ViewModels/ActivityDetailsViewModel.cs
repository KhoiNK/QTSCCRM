using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class ActivityDetailsViewModel
    {
        public ActivityDetailViewModel Activity { get; set; }
        public CustomerDetailViewModel Customer { get; set; }
        public ContactViewModel Contact { get; set; }
        public StaffDetailViewModel Staff { get; set; }

        public ActivityDetailsViewModel(Activity dto)
        {
            Activity = new ActivityDetailViewModel(dto);
            Customer = new CustomerDetailViewModel(dto.Customer);
            Contact = new ContactViewModel(dto.Contact);
            Staff = new StaffDetailViewModel(dto.Staff);

        }
    }
}