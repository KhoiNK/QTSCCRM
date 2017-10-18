using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class IssueDetailViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public DateTime? EstimateSolveEndDate { get; set; }
        public DateTime? ClosedDate { get; set; }


        public IssueDetailViewModel(Issue dto)
        {
            this.ID = dto.ID;
            this.Title = dto.Title;
            this.Description = dto.Description;
            this.Stage = dto.Stage;
            this.Status = dto.Status;
            this.EstimateSolveEndDate = dto.EstimateSolveEndDate;
            this.ClosedDate = dto.ClosedDate;
        }
    }
}