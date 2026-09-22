using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.Filter
{
    public class AdminQueryFilter
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? FullName { get; set; }
        public DateOnly? HireDate { get; set; }
    }
}
