using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data
{
    public class ProjectSeedData : DropCreateDatabaseIfModelChanges<APIProjectEntities>
    {
        protected override void Seed(APIProjectEntities context)
        {
            GetCategories().ForEach(c => context.Categories.Add(c));
            GetStaffs().ForEach(c => context.Staffs.Add(c));
            context.Commit();

            GetMarketingPlans().ForEach(c => context.MarketingPlans.Add(c));
            context.Commit();
        }
        private static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category {
                    Name = "Food",
                    isDelete = false,
                },
                new Category {
                    Name = "Drink",
                    isDelete = false,
                },
                new Category {
                    Name = "Snack",
                    isDelete = false,
                }
            };
        }

        private static List<Staff> GetStaffs()
        {
            return new List<Staff>
            {
                new Staff
                {
                    Name = "Tung"
                },
                new Staff
                {
                    Name = "Minh"
                },
                new Staff
                {
                    Name = "Duy"
                },
                new Staff
                {
                    Name ="Nhy"
                },
                new Staff
                {
                    Name = "Nhan"
                }
            };
        }

        private static List<MarketingPlan> GetMarketingPlans()
        {
            return new List<MarketingPlan>
            {
                new MarketingPlan
                {
                    CreateStaffID = 1,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao",
                    Stage = "Drafting",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 1,
                    LastModifiedDate = DateTime.Today.Date
                },
                new MarketingPlan
                {
                    CreateStaffID = 2,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    Stage = "Validating",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 2,
                    LastModifiedDate = DateTime.Today.Date
                },
                new MarketingPlan
                {
                    CreateStaffID = 3,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    Stage = "Approving",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 3,
                    LastModifiedDate = DateTime.Today.Date
                },
                new MarketingPlan
                {
                    CreateStaffID = 4,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    Stage = "Preparing",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 4,
                    LastModifiedDate = DateTime.Today.Date
                },
                new MarketingPlan
                {
                    CreateStaffID = 5,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    Stage = "Executing",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 5,
                    LastModifiedDate = DateTime.Today.Date
                },
            };
        }
    }
}
