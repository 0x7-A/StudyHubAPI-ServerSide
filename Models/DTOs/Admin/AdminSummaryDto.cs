namespace StudyHubAPI.Models.DTOs.Admin
{
 
        public class AdminSummaryDto
        {
            public int PersonID { get; set; }
            public string FullName { get; set; } = null!;
            public DateOnly HireDate { get; set; }
        
        }
}
