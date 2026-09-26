namespace StudyHubAPI.Models.Entities
{
    public class Countries
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public ICollection<Administrators> Administrators { get; set; } = new List<Administrators>();
    }
}
