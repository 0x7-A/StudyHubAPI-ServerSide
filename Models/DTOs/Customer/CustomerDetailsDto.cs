using StudyHubAPI.Models.DTOs.Person;
using System.Text.Json.Serialization;
using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Models.DTOs.Customer
{
    public class CustomerDetailsDto : PersonDetailsDto
    {
        [JsonIgnore]
        public override PersonRole Role { get; set; }
        public DateOnly RegisteredAt { get; set; }
    }
}
