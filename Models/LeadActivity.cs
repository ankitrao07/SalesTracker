namespace SalesTracker.Models
{
    public class LeadActivity
    {
        public int? LeadActivityId { get; set; }
        public int? LeadId { get; set; }
        public string? LeadStatus { get; set; }
        public string? LeadDate { get; set; }
        public string? NextAppointmentDate { get; set; }
        public bool IsReminderSet { get; set; } = false;
        public string? ReminderDate { get; set; }
        public string? MeetingMode { get; set; }
        public string? MeetingDate { get; set; }
        public string? FollowupDate { get; set; }
        public string? ResponseDesc { get; set; }
        public string? RemarkBP { get; set; }
        public string? RemarkSlf { get; set; }
        public string? RemarkSpl { get; set; }
        public string? UploadedFileName { get; set; }
        public string? AddedBy { get; set; }
        public string? AddedDate { get;set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedDate { get;set;}
    }

}
