namespace APIProject.Model.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MarketingResult")]
    public partial class MarketingResult:BaseEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int ID { get; set; }

        public int? CustomerID { get; set; }


        public int MarketingPlanID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public int FacilityRate { get; set; }
        public int ArrangingRate { get; set; }
        public int ServicingRate { get; set; }
        public int IndicatorRate { get; set; }
        public int OthersRate { get; set; }
        public bool IsFromMedia { get; set; }
        public bool IsFromInvitation { get; set; }
        public bool IsFromWebsite { get; set; }
        public bool IsFromFriend { get; set; }
        public bool IsFromOthers { get; set; }
        public bool IsWantMore { get; set; }
        public string Status { get; set; }
        public bool IsLeadGenerated { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("MarketingPlanID")]
        public virtual MarketingPlan MarketingPlan { get; set; }
    }
}
