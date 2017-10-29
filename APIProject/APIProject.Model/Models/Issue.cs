namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Issue")]
    public partial class Issue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Issue()
        {
            //SalesCategories = new HashSet<SalesCategory>();
            IssueCategoryMappings = new HashSet<IssueCategoryMapping>();

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? CustomerID { get; set; }

        public int? ContactID { get; set; }

        public int? CreateStaffID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public int? OpenStaffID { get; set; }
        public DateTime? OpenedDate { get; set; }
        public int? SolveStaffID { get; set; }
        public DateTime? SolveStartDate { get; set; }
        public DateTime? EstimateSolveEndDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int ExtendCount { get; set; }
        public string ExtendNote { get; set; }

        public int? AcceptStaffID { get; set; }

        public int? ModifiedStaffID { get; set; }
        public DateTime? ModifiedDate { get; set; }

        //public int? SalesCategoryID { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }

        //public virtual SalesCategory SalesCategory { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff OpenStaff { get; set; }

        public virtual Staff SolveStaff { get; set; }

        public virtual Staff AcceptStaff { get; set; }

        public virtual Staff ModifiedStaff { get; set; }

        
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SalesCategory> SalesCategories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IssueCategoryMapping> IssueCategoryMappings { get; set; }
    }
}
