namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesCategory")]
    public partial class SalesCategory:BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesCategory()
        {
            //Issues = new HashSet<Issue>();
            IssueCategoryMappings = new HashSet<IssueCategoryMapping>();
            OpportunityCategoryMappings = new HashSet<OpportunityCategoryMapping>();

            //SalesCategory1 = new HashSet<SalesCategory>();
            SalesItems = new HashSet<SalesItem>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int ID { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesItem> SalesItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IssueCategoryMapping> IssueCategoryMappings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OpportunityCategoryMapping> OpportunityCategoryMappings { get; set; }


    }
}
