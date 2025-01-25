using System.ComponentModel.DataAnnotations;
namespace SalesTracker.Models
{
    public class LeadDetail
    {
        public int? TrId { get; set; }
        public int? TrDetId { get; set; } // This is an identity column
        public string LeadStatus { get; set; } 
        public string? LeadDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string? RemDate { get; set; } 
        public string? RemarkBP { get; set; } 
        public string? RemarkSlf { get; set; } 
        public string? RemarkSpl { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string? AddDate { get; set; } 
        public string? AddBy { get; set; } 
        public string? UpdBy { get; set; }
        public string? UpdDate { get; set; }
    }
}
