using SalesTracker.Models.DTO;

namespace SalesTracker.Models
{
    public class vwLead_RecentActivities
    {
        public LeadCompositeDTO LeadComposite { get; set; }
        public List<LeadActivityDTO> RecentActivities { get; set; }
    }
}
