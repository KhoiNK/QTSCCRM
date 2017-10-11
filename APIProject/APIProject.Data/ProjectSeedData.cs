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
            GetCustomers().ForEach(c => context.Customers.Add(c));
            context.Commit();

            GetMarketingPlans().ForEach(c => context.MarketingPlans.Add(c));
            GetContacts().ForEach(c => context.Contacts.Add(c));
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
                    Stage = "Running",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                    ModifiedStaffID = 5,
                    LastModifiedDate = DateTime.Today.Date
                },
            };
        }

        private static List<Customer> GetCustomers()
        {
            return new List<Customer>
            {
                new Customer
                {
                    Name = "FPT Software",
                    Address = "39 Ông Địa",
                    IsLead = true
                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 2",
                    Address = "69 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Thường"
                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 3",
                    Address = "70 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Thân Thiết"
                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 4",
                    Address = "71 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Mẹ Thiên Hạ"
                }
            };
        }

        private static List<Contact> GetContacts()
        {
            return new List<Contact>
            {
                new Contact
                {
                    CustomerID = 1,
                    Name = "Anh Ba Một",
                    Email = "abm@abm.abm",
                    Phone = "111-111-111",
                },
                new Contact
                {
                    CustomerID = 2,
                    Name = "Anh Ba Hai",
                    Email = "abh@abh.abh",
                    Phone = "222-222-222",
                },
                new Contact
                {
                    CustomerID = 3,
                    Name = "Anh Ba Ba",
                    Email = "abb@abb.abb",
                    Phone = "333-333-333",
                },
                new Contact
                {
                    CustomerID = 4,
                    Name = "Anh Ba Bốn",
                    Email = "abb4@abb4.abb4",
                    Phone = "444-444-444",
                },
            };
        }
    }
}
