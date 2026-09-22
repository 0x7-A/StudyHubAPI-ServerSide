namespace StudyHubAPI.Models.Filter
{
    public class CustomerQueryFilter
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? FullName { get; set; }
        public DateOnly? RegisteredAt { get; set; }
    }
}
