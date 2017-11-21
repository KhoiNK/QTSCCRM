﻿using APIProject.Model.Models;
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
        public string Status { get; set; }
        public DateTime? SolveDate { get; set; }


        public IssueDetailViewModel(Issue dto)
        {
            this.ID = dto.ID;
            this.Title = dto.Title;
            this.Description = dto.Description;
            this.Status = dto.Status;
            this.SolveDate = dto.SolveDate;
        }
    }
}