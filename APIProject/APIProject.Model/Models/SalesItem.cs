namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesItem")]
    public partial class SalesItem:BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesItem()
        {
            QuoteItemMappings = new HashSet<QuoteItemMapping>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public int? SalesCategoryID { get; set; }

        [ForeignKey("SalesCategoryID")]
        public virtual SalesCategory SalesCategory { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuoteItemMapping> QuoteItemMappings { get; set; }

    }
}
