namespace SalesTracker.Utility
{
    public class Utility
    {
        public static string GenerateUniqueDocNo()
        {
            string dateTimePart = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomPart = Guid.NewGuid().ToString().Substring(0, 8); // Short random part
            return $"DOC-{dateTimePart}-{randomPart}";
        }
    }
}
