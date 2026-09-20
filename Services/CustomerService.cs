using StudyHubAPI.Models.DTOs.Customer;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;
namespace StudyHubAPI.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _CustomerRepository;
        private readonly PersonRepository _personRepository;

        public CustomerService(CustomerRepository customerRepository, PersonRepository personRepository)
        {
            _CustomerRepository = customerRepository;
            _personRepository = personRepository;
        }

        public async Task<int> AddCustomer(CreateCustomerDto dto)
        {
            // check email and phone is not taken.
            if (await _personRepository.IsEmailTaken(dto.Email))
            {
                return -1;
            }
            if (await _personRepository.IsPhoneNumberTaken(dto.PhoneNumber))
            {
                return -1;
            }

            var customer = new Customers
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                Role = PersonRole.Customer

            };

            // 2. Add via Customer DbSet / Repository only
            return await _CustomerRepository.AddCustomer(customer);
        }


        public async Task<CustomerDetailsDto?> GetCustomerByID(int PersonID)
        {
            var customer = await _CustomerRepository.GetCustomerByIdReadOnly(PersonID);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDetailsDto { PersonID = customer.PersonID,  FirstName = customer.FirstName,
                LastName = customer.LastName, Email = customer.Email, PhoneNumber = customer.PhoneNumber, RegisteredAt = customer.RegisteredAt};
        }


        public  Task<List<CustomerSummaryDto>> GetAllCustomers(int PageNumber,int PageSize)
        {
            return _CustomerRepository.GetAllCustomer(PageNumber, PageSize);
        }

        public async Task<bool> UpdateCustomer(int PersonId, UpdateCustomerDto dto)
        {
            var customer = await _CustomerRepository.GetCustomerByID(PersonId);

            if (customer == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                customer.FirstName = dto.FirstName;
            }

            if (!string.IsNullOrEmpty(dto.MiddleName))
            {
                customer.MiddleName = dto.MiddleName;
            }

            if (!string.IsNullOrEmpty(dto.LastName))
            {
                customer.LastName = dto.LastName;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                if (dto.PhoneNumber != customer.PhoneNumber && await _personRepository.IsPhoneNumberTaken(dto.PhoneNumber))
                {
                    return false;
                }

                customer.PhoneNumber = dto.PhoneNumber;
            }


            return await _personRepository.SaveChangeAsync() > 0;
        }


        public Task<bool> DeleteCustomerByID(int personID)
        {
            return _personRepository.SoftDeletePersonByIdAsync(personID);
        }






    }


}
