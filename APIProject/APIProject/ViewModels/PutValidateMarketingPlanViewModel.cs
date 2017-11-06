using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class PutValidateMarketingPlanViewModel
    {
        public int ID { get; set; }
        public int StaffID { get; set; }
        public bool Validate { get; set; }
        public string ValidateNotes { get; set; }

        public MarketingPlan ToMarketingPlanModel()
        {
            return new MarketingPlan
            {
                ID = this.ID,
            };
        }
    }
}