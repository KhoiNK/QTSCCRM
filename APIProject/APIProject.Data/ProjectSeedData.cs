using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using APIProject.GlobalVariables;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace APIProject.Data
{
    public class ProjectSeedData : DropCreateDatabaseIfModelChanges<APIProjectEntities>
    {
        protected override void Seed(APIProjectEntities context)
        {
            //GetCategories().ForEach(c => context.Categories.Add(c));
            List<Customer> customerList = GetCustomers();
            customerList.ForEach(c => context.Customers.Add(c));
            List<SalesCategory> categoryList = GetSalesCategories();
            categoryList.ForEach(c => context.SalesCategories.Add(c));
            List<Role> roleList = GetRoles();
            roleList.ForEach(c => context.Roles.Add(c));
            context.AppConfigs.Add(GetHostName());
            context.Commit();

            List<Contact> contactList = GetContacts(customerList);
            contactList.ForEach(c => context.Contacts.Add(c));
            List<Staff> staffList = GetStaffs(roleList);
            staffList.ForEach(c => context.Staffs.Add(c));
            context.Commit();

            GetMarketingPlans().ForEach(c => context.MarketingPlans.Add(c));
            List<SalesItem> salesItemList = GetSalesItem();
            salesItemList.ForEach(c => context.SalesItems.Add(c));
            List<Issue> issueList = GetIssues(contactList, staffList);
            issueList.ForEach(c => context.Issues.Add(c));
            context.Commit();

            GetIssueCategories(issueList).ForEach(c => context.IssueCategoryMappings.Add(c));
            List<Opportunity> opportunityList = GetOpportunities();
            opportunityList.ForEach(c => context.Opportunities.Add(c));
            context.Commit();
            //GetOppActivities(opportunityList).ForEach(c => context.Activities.Add(c));
            //List<Quote> quoteList = GetQuotes();
            //quoteList.ForEach(c => context.Quotes.Add(c));
            context.Commit();
            //GetQuoteItems(quoteList, salesItemList).ForEach(c => context.QuoteItemMappings.Add(c));
            GetOppCategories(opportunityList, categoryList).ForEach(c => context.OpportunityCategoryMappings.Add(c));
            context.Commit();

        }

        private List<IssueCategoryMapping> GetIssueCategories(List<Issue> issueList)
        {
            int i = 1;
            List<IssueCategoryMapping> _list = new List<IssueCategoryMapping>();
            foreach(var item in issueList)
            {
                for (int count = 1; count <= i; count++)
                {
                    _list.Add(new IssueCategoryMapping
                    {
                        IssueID = item.ID,
                        SalesCategoryID = count,
                        IsDeleted=false
                    });
                }
                if (i < 7)
                {
                    i++;
                }
                else
                {
                    i = 1;
                }
            }
            return _list;
        }

        private List<OpportunityCategoryMapping> GetOppCategories(List<Opportunity> oppList, List<SalesCategory> categoryList)
        {
            List<OpportunityCategoryMapping> _list = new List<OpportunityCategoryMapping>();
            foreach (Opportunity oppItem in oppList)
            {
                foreach (SalesCategory category in categoryList)
                {
                    _list.Add(new OpportunityCategoryMapping
                    {
                        IsDeleted = false,
                        OpportunityID = oppItem.ID,
                        SalesCategoryID = category.ID
                    });
                }
            }
            return _list;
        }
        private List<QuoteItemMapping> GetQuoteItems(List<Quote> quoteList, List<SalesItem> itemList)
        {
            List<QuoteItemMapping> _list = new List<QuoteItemMapping>();
            foreach (SalesItem salesItem in itemList)
            {
                foreach (Quote quoteItem in quoteList)
                {
                    _list.Add(new QuoteItemMapping
                    {
                        QuoteID = quoteItem.ID,
                        SalesItemID = salesItem.ID,
                        Price = salesItem.Price,
                        SalesItemName = salesItem.Name,
                        Unit = salesItem.Unit
                    });
                }
            }

            return _list;
        }

        private static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    Name = RoleName.Admin
                },
                new Role
                {
                    Name = RoleName.Director
                },
                new Role
                {
                    Name = RoleName.Marketing
                },
                new Role
                {
                    Name = RoleName.Officer
                },
                new Role
                {
                    Name = RoleName.Sales
                },
                new Role
                {
                    Name = RoleName.Support
                },
            };
        }

        private static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category {
                    Name = "Food",
                    IsDelete = false,
                },
                new Category {
                    Name = "Drink",
                    IsDelete = false,
                },
                new Category {
                    Name = "Snack",
                    IsDelete = false,
                }
            };
        }

        private List<Staff> GetStaffs(List<Role> roleList)
        {
            List<Staff> _list = new List<Staff>();
            int i = 1;
            foreach (Role item in roleList)
            {
                _list.Add(new Staff
                {
                    Name = "Nguyễn Văn " + i,
                    Phone = i + i + i + "-" + i + i + i + "-" + i + i + i,
                    Email = i + i + i + "@" + i + i + i + "." + i + i + i,
                    RoleID = item.ID,
                });
                i++;
            }
            return _list;
        }

        private static AppConfig GetHostName()
        {
            return new AppConfig
            {
                Name = "Host",
                Value = "http://localhost:50198"
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
            string imgName = "a11f666d-104b-45f5-9442-443b004c915a_20171018204923.jpg";
            List<Customer> _list = new List<Customer>();
            for (int i = 1; i <= 2; i++)
            {
                _list.Add(new Customer
                {
                    Name = "FPT Software " + i,
                    Address = i + " Ông Địa",
                    CustomerType = CustomerType.Lead,
                    EstablishedDate = DateTime.Today.Date,
                    AvatarSrc = imgName,
                    TaxCode = i.ToString()+i+i+i+i+i+i+i+i+i
                });
            }
            for (int i = 3; i <= 4; i++)
            {
                _list.Add(new Customer
                {
                    Name = "FPT Software " + i,
                    Address = i + " Ông Địa",
                    CustomerType = CustomerType.Official,
                    EstablishedDate = DateTime.Today.Date,
                    TaxCode = i.ToString()+i+i+i+i+i+i+i+i+i,
                    AvatarSrc = imgName
                });
            }
            for (int i = 5; i <= 6; i++)
            {
                _list.Add(new Customer
                {
                    Name = "FPT Software " + i,
                    Address = i + " Ông Địa",
                    CustomerType = CustomerType.Inside,
                    EstablishedDate = DateTime.Today.Date,
                    TaxCode = i.ToString()+i+i+i+i+i+i+i+i+i,
                    AvatarSrc = imgName
                });
            }
            for (int i = 7; i <= 8; i++)
            {
                _list.Add(new Customer
                {
                    Name = "FPT Software " + i,
                    Address = i + " Ông Địa",
                    CustomerType = CustomerType.Outside,
                    EstablishedDate = DateTime.Today.Date,
                    TaxCode = i.ToString()+i+i+i+i+i+i+i+i+i,
                    AvatarSrc = imgName
                });
            }

            return _list;
        }

        private List<Contact> GetContacts(List<Customer> customerList)
        {
            string imgSrc = "e8c2fa60-2af6-48dc-b2db-18cfa2be60be_20171019104211.jpg";
            List<Contact> _list = new List<Contact>();
            int count = 1;
            foreach (Customer customer in customerList)
            {

                _list.Add(new Contact
                {
                    CustomerID = customer.ID,
                    Name = "Nguyễn Văn " + count,
                    Email = count.ToString() + count + count + "@gmail.com",
                    Phone = count.ToString() + count + count + "-" +
                    count + count + count + "-" +
                    count + count + count,
                    AvatarSrc = imgSrc,
                    Position = "Nhân viên thứ " + count
                });
                count++;
            }
            return _list;
        }

        private static List<Issue> GetIssues(List<Contact> contactList, List<Staff> staffList)
        {
            List<Issue> _list = new List<Issue>();
            int count = 1;
            foreach (var staff in staffList)
            {
                if (staff.Role.Name == RoleName.Support)
                {
                    foreach (var contact in contactList)
                    {
                        if (contact.Customer.CustomerType != CustomerType.Lead)
                        {
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Stage = IssueStage.Open,
                                Status = IssueStatus.Open,
                                EstimateSolveEndDate = null
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Stage = IssueStage.Solving,
                                Status = IssueStatus.Doing,
                                EstimateSolveEndDate = DateTime.Now
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Stage = IssueStage.Solving,
                                Status = IssueStatus.Overdue,
                                EstimateSolveEndDate = DateTime.Now
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Stage = IssueStage.Closed,
                                Status = IssueStatus.Done,
                                EstimateSolveEndDate = DateTime.Now,
                                ClosedDate = DateTime.Today.Date
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Stage = IssueStage.Closed,
                                Status = IssueStatus.Failed,
                                EstimateSolveEndDate = DateTime.Now,
                                ClosedDate = DateTime.Today.Date
                            });

                        }
                    }
                }
            }

            return _list;
            
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
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
                OpportunityStage.Negotiation,
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
                        CreatedStaffID = i,
                        Title = "Cơ hội " + count++,
                        StageName = stage
                    });
                }
            }
            return list;
        }

        private static List<Quote> GetQuotes()
        {
            List<Quote> list = new List<Quote>();
            List<Opportunity> oppList = GetOpportunities();
            int i = 1;
            foreach (Opportunity oppItem in oppList)
            {
                if (oppItem.StageName == OpportunityStage.MakeQuote)
                {
                    list.Add(new Quote
                    {
                        OpportunityID = i,
                        Tax = 10,
                        Discount = 10,
                        Status = QuoteStatus.NotValid
                    });
                    list.Add(new Quote
                    {
                        OpportunityID = i,
                        Tax = 10,
                        Discount = 10,
                        Status = QuoteStatus.Drafting
                    });
                }
                if (oppItem.StageName == OpportunityStage.ValidateQuote)
                {
                    list.Add(new Quote
                    {
                        OpportunityID = i,
                        Tax = 10,
                        Discount = 10,
                        Status = QuoteStatus.Validating
                    });
                }
                if (oppItem.StageName == OpportunityStage.SendQuote
                    || oppItem.StageName == OpportunityStage.Negotiation
                    || oppItem.StageName == OpportunityStage.Won
                    || oppItem.StageName == OpportunityStage.Lost)
                {
                    list.Add(new Quote
                    {
                        OpportunityID = i,
                        Tax = 10,
                        Discount = 10,
                        Status = QuoteStatus.Valid
                    });
                }
                i++;
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

        private List<Activity> GetOppActivities(List<Opportunity> oppList)
        {
            int i = 1;
            List<Activity> _list = new List<Activity>();
            foreach (Opportunity item in oppList)
            {
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.Consider
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.MakeQuote
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.Negotiation
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.SendQuote
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.ValidateQuote
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.Won
                });
                i++;
                _list.Add(new Activity
                {
                    OpportunityID = item.ID,
                    CustomerID = item.CustomerID,
                    ContactID = item.ContactID,
                    CreateStaffID = item.CreatedStaffID,
                    Type = ActivityType.ToCustomer,
                    Method = ActivityMethod.Direct,
                    Title = "Bàn về vấn đề: " + i,
                    Description = "Chi tiết về vấn đề " + i,
                    OpporunityGenerated = false,
                    TodoTime = DateTime.Now,
                    Status = ActivityStatus.Completed,
                    OfOpportunityStage = OpportunityStage.Lost
                });
                i++;
            }
            return _list;
        }
    }
}
