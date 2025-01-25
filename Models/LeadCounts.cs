namespace SalesTracker.Models
{
    public class LeadCounts
    {
        public int TotalLeads { get; set; }
        public int ActiveLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int TodaysActionableLeads
        {
            get; set;
        }
    }
}

