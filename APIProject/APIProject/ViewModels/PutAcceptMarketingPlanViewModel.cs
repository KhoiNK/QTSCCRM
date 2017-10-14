using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutAcceptMarketingPlanViewModel
    {
        public int ID { get; set; }
        public int StaffID { get; set; }
        public bool Accept { get; set; }
        public string AcceptNotes { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            return new MarketingPlan
            {
                ID = this.ID,
                ModifiedStaffID = this.StaffID,
                AcceptNotes = this.AcceptNotes,
            };
        }
    }
}