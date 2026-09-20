using StudyHubAPI.Models.DTOs.Person;
using System.Text.Json.Serialization;

namespace StudyHubAPI.Models.DTOs.Customer
{
    public class CreateCustomerDto : CreatePersonDto
    {
        [JsonIgnore]
        public  byte Role { get; set; }
       
    }
}
