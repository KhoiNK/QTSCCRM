namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuoteItemMapping")]
    public partial class QuoteItemMapping:BaseEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int ID { get; set; }
        public int QuoteID { get; set; }
        public int SalesItemID { get; set; }
        public string SalesItemName { get; set; }
        public int? Price { get; set; }
        public string Unit { get; set; }
        [ForeignKey("QuoteID")]
        public virtual Quote Quote { get; set; }
        [ForeignKey("SalesItemID")]
        public virtual SalesItem SalesItem { get; set; }
    }
}
