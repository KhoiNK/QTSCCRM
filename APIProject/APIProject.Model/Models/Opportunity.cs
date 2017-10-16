namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Opportunity")]
    public partial class Opportunity
    {
        public Opportunity()
        {
            Activities = new HashSet<Activity>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? CustomerID { get; set; }

        public int? ContactID { get; set; }

        public int? CreateStaffID { get; set; }

        public int? ModifyStaffID { get; set; }
        public string Title { get; set; }
        public string Stage { get; set; }
        public string Description { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Staff Staff1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
