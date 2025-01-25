namespace SalesTracker.Models.DTO
{
    public class LeadDTO
    {

        public int? LeadId { get; set; }
        public string? LeadNumber { get; set; }
        public string? DocNo { get; set; }
        public string? DocDate { get; set; }
        public string? EnteredBy { get; set; }
        public string? LeadSource { get; set; }
        public string? LeadOwner { get; set; }
        public string? LeadOwnerContactNo { get; set; }
        public string? LeadPriority { get; set; }
        public string? LeadTitle { get; set; }
        public string? LeadCategory { get; set; }
        public string? LeadDesc { get; set; }
        public string? SpecialRequirement { get; set; }
        public string? PotentialDealValue { get; set; }
        public string? ProbabilityOfConversion { get; set; }
        public string? ClosureForecast { get; set; }
        public string? CompanyName { get; set; }
        public string? BusinessSector { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNo { get; set; }
        public string? Designation { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? PEmail { get; set; }
        public string? Remark { get; set; }
        public string? CurrentStatus { get; set; }
        public string? NextReminderDate { get; set; }
        public string? LeadDate { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? StateName { get; set; }
        public string? Zip { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? AddedBy { get; set; } = "Ankit Kumar";
        public string? UpdatedBy { get; set; } = "Ankit Kumar";
        public string? UpdatedDate { get; set; }
        public bool? IsOverdue { get; set; }

    }
}
