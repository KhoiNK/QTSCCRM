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
            context.AppConfigs.Add(GetContractItemCode());
            context.AppConfigs.Add(GetContractCode());
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

            //GetIssueCategories(issueList).ForEach(c => context.IssueCategoryMappings.Add(c));
            List<Opportunity> opportunityList = GetOpportunities();
            opportunityList.ForEach(c => context.Opportunities.Add(c));
            context.Commit();
            GetOppActivities(opportunityList).ForEach(c => context.Activities.Add(c));
            List<Quote> quoteList = GetQuotes();
            quoteList.ForEach(c => context.Quotes.Add(c));
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
                    });
                }
                if (i < 6)
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
                        IsDelete = false,
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
                Value = "http://crmcp.azurewebsites.net"
            };
        }

        private static AppConfig GetContractCode()
        {
            return new AppConfig
            {
                Name = "ContractCode",
                Value = "HD"
            };
        }

        private static AppConfig GetContractItemCode()
        {
            return new AppConfig
            {
                Name = "ContractItemCode",
                Value = "CT"
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
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                },
                new MarketingPlan
                {
                    CreateStaffID = 2,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                },
                new MarketingPlan
                {
                    CreateStaffID = 3,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                },
                new MarketingPlan
                {
                    CreateStaffID = 4,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
                },
                new MarketingPlan
                {
                    CreateStaffID = 5,
                    CreatedDate = DateTime.Today.Date,
                    Title = "Quang cao ban son",
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date,
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
                                Status = IssueStatus.Open,
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Status = IssueStatus.Doing,
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Status = IssueStatus.Overdue,
                            });
                            count++;
                            _list.Add(new Issue
                            {
                                CreateStaffID = staff.ID,
                                CustomerID = contact.Customer.ID,
                                ContactID = contact.ID,
                                Title = "Hư chỗ " + count,
                                Description = "Chi tiết việc hư chỗ " + count,
                                Status = IssueStatus.Done,
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
                                Status = IssueStatus.Failed,
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
                    Name = "Dịch vụ viễn thông",
                },
                new SalesCategory
                {
                    Name = "Dịch vụ thuê văn phòng",
                }
            };
        }

        private static List<SalesItem> GetSalesItem()
        {
            return new List<SalesItem>
            {
                new SalesItem
                {
                    SalesCategoryID=2,
                    Name = "Văn phòng hạng A",
                    Price = 50000000,
                    Unit="Tháng"
                },
                new SalesItem
                {
                    SalesCategoryID=2,
                    Name = "Văn phòng hạng B",
                    Price = 40000000,
                    Unit="Tháng"
                },
                new SalesItem
                {
                    SalesCategoryID=2,
                    Name = "Văn phòng hạng ",
                    Price = 30000000,
                    Unit="Tháng"
                },
                new SalesItem
                {
                    SalesCategoryID=1,
                    Name = "Data center",
                    Price = 10000000,
                    Unit="Tháng"
                },
                new SalesItem
                {
                    SalesCategoryID=1,
                    Name = "Kết nối internet",
                    Price = 10000000,
                    Unit="Tháng"
                }

            };
        }

        private static List<Opportunity> GetOpportunities()
        {
            List<Opportunity> list = new List<Opportunity>();
            List<string> stages = new List<string>
            {
                OpportunityStage.Consider
                //OpportunityStage.MakeQuote,
                //OpportunityStage.ValidateQuote,
                //OpportunityStage.SendQuote,
                //OpportunityStage.Negotiation,
            };
            int count = 1;
            foreach (string stage in stages)
            {
                for (int i = 1; i <= 1; i++)
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
                        CreatedStaffID=1,
                        Tax = 10,
                        Discount = 10,
                        Status = QuoteStatus.Drafting
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
