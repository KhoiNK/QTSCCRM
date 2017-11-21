using APIProject.Data.Configuration;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data
{
    public class APIProjectEntities : DbContext
    {
        public APIProjectEntities() : base("APIProjectEntities") { }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<MarketingPlan> MarketingPlans { get; set; }
        public virtual DbSet<MarketingResult> MarketingResults { get; set; }
        public virtual DbSet<Opportunity> Opportunities { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SalesCategory> SalesCategories { get; set; }
        public virtual DbSet<SalesItem> SalesItems { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<IssueCategoryMapping> IssueCategoryMappings { get; set; }
        public virtual DbSet<OpportunityCategoryMapping> OpportunityCategoryMappings { get; set; }
        public virtual DbSet<QuoteItemMapping> QuoteItemMappings { get; set; }
        public virtual DbSet<Quote> Quotes { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<AppConfig> AppConfigs { get; set; }
        //public virtual DbSet<Stage> Stages { get; set; }




        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<SalesCategory>()
            //      .HasMany(e => e.SalesCategory1)
            //      .WithOptional(e => e.SalesCategory2)
            //      .HasForeignKey(e => e.OfCategoryID);

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.Activities)
                .WithOptional(e => e.Staff)
                .HasForeignKey(e => e.CreateStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Activities1)
            //    .WithOptional(e => e.Staff1)
            //    .HasForeignKey(e => e.OfStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Activities2)
            //    .WithOptional(e => e.Staff2)
            //    .HasForeignKey(e => e.ModifiedStaffID);

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.Issues)
                .WithOptional(e => e.Staff)
                .HasForeignKey(e => e.CreateStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Issues1)
            //    .WithOptional(e => e.OpenStaff)
            //    .HasForeignKey(e => e.OpenStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Issues2)
            //    .WithOptional(e => e.SolveStaff)
            //    .HasForeignKey(e => e.SolveStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Issues3)
            //    .WithOptional(e => e.AcceptStaff)
            //    .HasForeignKey(e => e.AcceptStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Issues4)
            //    .WithOptional(e => e.ModifiedStaff)
            //    .HasForeignKey(e => e.ModifiedStaffID);

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.MarketingPlans)
                .WithOptional(e => e.CreateStaff)
                .HasForeignKey(e => e.CreateStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.MarketingPlans1)
            //    .WithOptional(e => e.ModifiedStaff)
            //    .HasForeignKey(e => e.ModifiedStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.MarketingPlans2)
            //    .WithOptional(e => e.ValidateStaff)
            //    .HasForeignKey(e => e.ValidateStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.MarketingPlans3)
            //    .WithOptional(e => e.AcceptStaff)
            //    .HasForeignKey(e => e.AcceptStaffID);



            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.UpdatedOpportunities)
            //    .WithOptional(e => e.UpdatedStaff)
            //    .HasForeignKey(e => e.UpdatedStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.CreatedOpportunities)
            //    .WithOptional(e => e.CreatedStaff)
            //    .HasForeignKey(e => e.CreatedStaffID);

            //modelBuilder.Entity<Staff>()
            //    .HasMany(e => e.Quotes)
            //    .WithOptional(e => e.CreatedStaff)
            //    .HasForeignKey(e => e.CreatedStaffID);

            modelBuilder.Entity<Staff>()
               .HasMany(e => e.ValidateQuotes)
               .WithOptional(e => e.ValidatedStaff)
               .HasForeignKey(e => e.ValidatedStaffID);

            //modelBuilder.Entity<Issue>()
            //    .HasMany(e => e.Issues)
            //    .WithMany(e => e.SalesCategories)
            //    .Map(m => m.ToTable("IssueSalesCategoryMapping").MapLeftKey("SalesCategoryID").MapRightKey("IssueID"));
            //modelBuilder.Entity<Issue>()
            //    .HasMany(e => e.SalesCategories)
            //    .WithMany(e => e.Issues)
            //    .Map(m => m.ToTable("IssueCategoryMapping").MapLeftKey("IssueID").MapRightKey("SalesCategoryID"));
        }
    }
}
