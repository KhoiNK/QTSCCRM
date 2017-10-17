namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuoteItemMapping")]
    public partial class QuoteItemMapping
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuoteID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SalesItemID { get; set; }

        public int? Price { get; set; }

        public string Unit { get; set; }

        public virtual Quote Quote { get; set; }

        public virtual SalesItem SalesItem { get; set; }
    }
}
