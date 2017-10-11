using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class MarketingPlanViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Budget { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Stage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedStaffName { get; set; }
    }
}