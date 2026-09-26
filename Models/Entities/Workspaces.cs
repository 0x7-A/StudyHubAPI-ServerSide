namespace StudyHubAPI.Models.Entities
{
     public class Workspaces
     {
            public int WorkspaceID { get; set; }

            public string Description { get; set; } = null!;

            public byte WorkspaceStatus { get; set; }

            public decimal HourlyRate { get; set; }

            public int MaximumCapacity { get; set; }
            public ICollection<Reservations> Reservations { get; set; }
                = new HashSet<Reservations>();
            public ICollection<WorkspaceImages> WorkspaceImages { get; set; }
                = new HashSet<WorkspaceImages>();
     }
    
}
