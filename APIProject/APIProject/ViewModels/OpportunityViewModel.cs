using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class OpportunityViewModel
    {
        public int ID { get; set; }
        public string AvatarSrc { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public string StageName { get; set; }
        public DateTime? NextActivityTime { get; set; }
        public string Owner { get; set; }
        public OpportunityViewModel(Opportunity oppDto,
            Customer customerDto,
            Activity activityDto,
            Staff staffDto)
        {
            this.ID = oppDto.ID;
            this.AvatarSrc = customerDto.AvatarSrc;
            this.CustomerName = customerDto.Name;
            this.Title = oppDto.Title;
            this.StageName = oppDto.StageName;
            if (activityDto != null)
            {
                NextActivityTime = activityDto.TodoTime.Value;
            }
            Owner = staffDto.Name;
        }
        public OpportunityViewModel(Opportunity dto)
        {
            this.ID = dto.ID;
            this.AvatarSrc = dto.Customer.AvatarSrc;
            this.CustomerName = dto.Customer.Name;
            this.Title = dto.Title;
            this.StageName = dto.StageName;
            var foundLastOngoingActivity = dto.Activities.Where(c => c.Type == ActivityType.ToCustomer
            && (c.Status == ActivityStatus.Open || c.Status == ActivityStatus.Overdue)).LastOrDefault();
            if (foundLastOngoingActivity != null)
            {
                NextActivityTime = foundLastOngoingActivity.TodoTime.Value;
            }
            Owner = dto.CreatedStaff.Name;
        }
    }

    public class PutOpportunityInformationViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<int> CategoryIDs { get; set; }

        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                UpdatedStaffID = this.StaffID,
                Title = this.Title,
                Description = this.Description
            };
        }
    }
    public class PutOpportunityInformationResponseViewModel
    {
        public bool BasicInfoUpdated { get; set; }
        public bool CategoriesUpdated { get; set; }
    }

    public class PutWonOpportunityViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                UpdatedStaffID = this.StaffID
            };
        }
    }
    public class PutWonOppResponseViewModel
    {
        public bool OpportunityUpdated { get; set; }
        public bool CustomerConverted { get; set; }
    }

    public class PutLostOpportunityViewModdel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                UpdatedStaffID = this.StaffID
            };
        }
    }
    public class PutLostOppResponseViewModel
    {
        public bool OpportunityUpdated { get; set; }
    }

    public class PutOpportunityNextStageViewModel
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int StaffID { get; set; }

        public Opportunity ToOpportunityModel()
        {
            return new Opportunity
            {
                ID = this.ID,
                UpdatedStaffID = this.StaffID
            };
        }
    }
    public class PutOpportunityNextStageResponseViewModel
    {
        public bool OpportunityUpdated { get; set; }
        public bool QuoteUpdated { get; set; }
    }

}