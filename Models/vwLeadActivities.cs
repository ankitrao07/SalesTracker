using System.Diagnostics;

namespace SalesTracker.Models
{
    public class vwLeadActivities
    {
        public vwLeadDetail vwLeadDetail { get; set; }
        public List<LeadDetail> RecentActivities { get; set; }
    }
}
