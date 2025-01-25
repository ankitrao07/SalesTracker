namespace SalesTracker.Models.DTO
{
    public class LeadCompositeDTO
    {
        public LeadDTO Lead { get; set; }
        public LeadActivityDTO LeadActivity { get; set; }
    }

}
