namespace StudyHubAPI.Models.DTOs.Customer
{
    public class CustomerSummaryDto
    {
        public int PersonID { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly RegisteredAt { get; set; }

    }
}
