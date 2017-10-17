using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using APIProject.GlobalVariables;
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
            GetSalesCategories().ForEach(c => context.SalesCategories.Add(c));
            context.Commit();

            GetMarketingPlans().ForEach(c => context.MarketingPlans.Add(c));
            GetContacts().ForEach(c => context.Contacts.Add(c));
            GetSalesItem().ForEach(c => context.SalesItems.Add(c));
            GetIssues().ForEach(c => context.Issues.Add(c));
            context.Commit();
            GetActivities().ForEach(c => context.Activities.Add(c));
            GetOpportunities().ForEach(c => context.Opportunities.Add(c));
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
                    Name = "Tùng",
                    Phone = "123-123-123",
                    Email ="123@123.123"
                },
                new Staff
                {
                    Name = "Minh",
                    Phone = "123-123-123",
                    Email ="123@123.123"
                },
                new Staff
                {
                    Name = "Duy",
                    Phone = "123-123-123",
                    Email ="123@123.123"
                },
                new Staff
                {
                    Name ="Nhy",
                    Phone = "123-123-123",
                    Email ="123@123.123"
                },
                new Staff
                {
                    Name = "Nhân",
                    Phone = "123-123-123",
                    Email ="123@123.123"
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
                    IsLead = true,
                    EstablishedDate = DateTime.Today.Date
                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 2",
                    Address = "69 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Khách hàng nội",
                    EstablishedDate = DateTime.Today.Date

                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 3",
                    Address = "70 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Khách hàng ngoại",
                    EstablishedDate = DateTime.Today.Date

                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 4",
                    Address = "71 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Khách hàng nội",
                    EstablishedDate = DateTime.Today.Date
                },
                new Customer
                {
                    Name = "FPT Software Cơ Sở 5",
                    Address = "72 Ông Địa",
                    IsLead = false,
                    ConvertedDate = DateTime.Today.Date,
                    CustomerType = "Khách hàng ngoại",
                    EstablishedDate = DateTime.Today.Date
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

        private static List<Issue> GetIssues()
        {
            return new List<Issue>
            {
                new Issue
                {
                    CreateStaffID=1,
                    CustomerID=1,
                    ContactID=1,
                    Title="Hư phần A",
                    OpenedDate=DateTime.Today.Date,
                    Stage= "Chưa xử lý"
                },
                new Issue
                {
                    CreateStaffID=2,
                    CustomerID=2,
                    ContactID=2,
                    Title="Hư phần B",
                    OpenedDate=DateTime.Today.Date,
                    Stage= "Đang xử lý"
                },
                new Issue
                {
                    CreateStaffID=3,
                    CustomerID=3,
                    ContactID=3,
                    Title="Hư phần C",
                    OpenedDate=DateTime.Today.Date,
                    Stage= "Hoàn thành"
                },
                new Issue
                {
                    CreateStaffID=4,
                    CustomerID=4,
                    ContactID=4,
                    Title="Hư phần D",
                    OpenedDate=DateTime.Today.Date,
                    Stage= "Hoàn thành"
                },
            };
        }

        private static List<SalesCategory> GetSalesCategories()
        {
            return new List<SalesCategory>
            {
                new SalesCategory
                {
                    Name = "Hội trường, phòng họp",
                },
                new SalesCategory
                {
                    Name = "ATM",
                },
                new SalesCategory
                {
                    Name = "Bãi đậu xe",
                },
                new SalesCategory
                {
                    Name = "Ký túc xá",
                },
                new SalesCategory
                {
                    Name = "Pano quảng cáo",
                },
                new SalesCategory
                {
                    Name = "Quay phim - quảng cáo",
                },
                new SalesCategory
                {
                    Name = "Khác"
                }
            };
        }

        private static List<SalesItem> GetSalesItem()
        {
            return new List<SalesItem>
            {
                new SalesItem
                        {
                            SalesCategoryID=6,
                            Name = "Vị trí A",
                            Price = 500000,
                            Unit = "ngày"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=6,
                            Name = "Vị trí B",
                            Price = 500000,
                            Unit = "ngày"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=6,
                            Name = "Vị trí C",
                            Price = 500000,
                            Unit = "ngày"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=6,
                            Name = "Vị trí D",
                            Price = 500000,
                            Unit = "ngày"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=6,
                            Name = "Vị trí E",
                            Price = 500000,
                            Unit = "ngày"
                        },
                new SalesItem
                        {
                            SalesCategoryID=5,
                            Name = "Vị trí A",
                            Price = 150000,
                            Unit = "pano/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=5,
                            Name = "Vị trí B",
                            Price = 150000,
                            Unit = "pano/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=5,
                            Name = "Vị trí C",
                            Price = 150000,
                            Unit = "pano/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=5,
                            Name = "Vị trí D",
                            Price = 150000,
                            Unit = "pano/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=5,
                            Name = "Vị trí E",
                            Price = 150000,
                            Unit = "pano/tháng"
                        },
                new SalesItem
                        {
                            SalesCategoryID=4,
                            Name = "Vị trí A",
                            Price = 3000000,
                            Unit = "phòng/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=4,
                            Name = "Vị trí B",
                            Price = 3000000,
                            Unit = "phòng/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=4,
                            Name = "Vị trí C",
                            Price = 3000000,
                            Unit = "phòng/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=4,
                            Name = "Vị trí D",
                            Price = 3000000,
                            Unit = "phòng/tháng"
                        },
                        new SalesItem
                        {
                            SalesCategoryID=4,
                            Name = "Vị trí E",
                            Price = 3000000,
                            Unit = "phòng/tháng"
                        },
                new SalesItem
                        {

                    SalesCategoryID=1,
                            Name = "Hội trường A",
                            Price = 1000000,
                            Unit = "Phòng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=1,
                            Name = "Hội trường B",
                            Price = 1000000,
                            Unit = "Phòng"

                        },
                        new SalesItem
                        {
                    SalesCategoryID=1,
                            Name = "Hội trường C",
                            Price = 1000000,
                            Unit = "Phòng"

                        },
                        new SalesItem
                        {
                    SalesCategoryID=1,
                            Name = "Hội trường D",
                            Price = 1000000,
                            Unit = "Phòng"

                        },
                        new SalesItem
                        {
                    SalesCategoryID=1,
                            Name = "Hội trường E",
                            Price = 1000000,
                            Unit = "Phòng"

                        },

                        new SalesItem
                        {
                    SalesCategoryID=2,

                            Name = "Vị trí A",
                            Price = 1000000,
                            Unit = "máy/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=2,
                            Name = "Vị trí B",
                            Price = 1000000,
                            Unit = "máy/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=2,
                            Name = "Vị trí C",
                            Price = 1000000,
                            Unit = "máy/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=2,
                            Name = "Vị trí D",
                            Price = 1000000,
                            Unit = "máy/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=2,
                            Name = "Vị trí E",
                            Price = 1000000,
                            Unit = "máy/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=3,
                            Name = "Vị trí A",
                            Price = 150000,
                            Unit = "xe/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=3,
                            Name = "Vị trí B",
                            Price = 150000,
                            Unit = "xe/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=3,
                            Name = "Vị trí C",
                            Price = 150000,
                            Unit = "xe/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=3,
                            Name = "Vị trí D",
                            Price = 150000,
                            Unit = "xe/tháng"
                        },
                        new SalesItem
                        {
                    SalesCategoryID=3,
                            Name = "Vị trí E",
                            Price = 150000,
                            Unit = "xe/tháng"
                        }
            };
        }

        private static List<Opportunity> GetOpportunities()
        {
            List<Opportunity> list = new List<Opportunity>();
            List<string> stages = new List<string>
            {
                OpportunityStage.Open,
                OpportunityStage.Consider,
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
                OpportunityStage.Negotiation,
                OpportunityStage.Won,
                OpportunityStage.Lost
            };
            int count = 1;
            foreach (string stage in stages)
            {
                for (int i = 1; i <= 4; i++)
                {
                    list.Add(new Opportunity
                    {
                        ContactID = i,
                        CustomerID = i,
                        CreateStaffID = i,
                        ModifyStaffID = i,
                        Title = "Cơ hội " + count++,
                        StageName = stage
                    });
                }
            }
            return list;
        }
        

        private static List<Activity> GetActivities()
        {
            return new List<Activity>
            {
                new Activity
                {
                    CreateStaffID=1,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=1,
                    ContactID=1,
                    Title="Bàn vụ A",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Email,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Open
                },
                new Activity
                {
                    CreateStaffID=2,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=2,
                    ContactID=2,
                    Title="Bàn vụ B",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Email,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Completed
                },
                new Activity
                {
                    CreateStaffID=3,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=3,
                    ContactID=3,
                    Title="Bàn vụ C",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Email,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Overdue
                },
                new Activity
                {
                    CreateStaffID=4,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=4,
                    ContactID=4,
                    Title="Bàn vụ D",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Email,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Overdue
                },
                new Activity
                {
                    CreateStaffID=1,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=2,
                    ContactID=2,
                    Title="Bàn vụ E",
                    Type=ActivityType.FromCustomer,
                    Method=ActivityMethod.Phone,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Recorded
                },
                new Activity
                {
                    CreateStaffID=1,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=3,
                    ContactID=3,
                    Title="Bàn vụ F",
                    Type=ActivityType.FromCustomer,
                    Method=ActivityMethod.Direct,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Recorded
                },
                new Activity
                {
                    CreateStaffID=1,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=4,
                    ContactID=4,
                    Title="Bàn vụ F",
                    Type=ActivityType.FromCustomer,
                    Method=ActivityMethod.Direct,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Recorded
                },
                new Activity
                {
                    CreateStaffID=2,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=4,
                    ContactID=4,
                    Title="Bàn vụ G",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Direct,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Open
                },
                new Activity
                {
                    CreateStaffID=3,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=4,
                    ContactID=4,
                    Title="Bàn vụ H",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Email,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Canceled
                },
                new Activity
                {
                    CreateStaffID=3,
                    CreatedDate=DateTime.Today.Date,
                    CustomerID=4,
                    ContactID=4,
                    Title="Bàn vụ I",
                    Type=ActivityType.ToCustomer,
                    Method=ActivityMethod.Phone,
                    TodoTime=DateTime.Now,
                    Status=ActivityStatus.Completed
                },
            };
        }
    }
}
