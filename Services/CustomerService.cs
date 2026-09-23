using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Customer;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
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

        public async Task<ServiceResult<int>> AddCustomer(CreateCustomerDto dto)
        {
            if (await _personRepository.IsEmailTaken(dto.Email))
            {
                return ServiceResult<int>.Failure(400, "Email is already taken.");   
            }
            if (await _personRepository.IsPhoneNumberTaken(dto.PhoneNumber))
            {
                return ServiceResult<int>.Failure(400, "Phone number is already taken.");
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

           return ServiceResult<int>.Success(await _CustomerRepository.AddCustomer(customer), 201);
        }


        public async Task<ServiceResult<CustomerDetailsDto?>> GetCustomerByID(int PersonID)
        {
            var customer = await _CustomerRepository.GetCustomerByIdReadOnly(PersonID);

            if (customer == null)
            {
                return ServiceResult<CustomerDetailsDto?>.Failure(404, "Customer not found.");
            }

            return ServiceResult<CustomerDetailsDto?>.Success(new CustomerDetailsDto { PersonID = customer.PersonID,  FirstName = customer.FirstName,
                LastName = customer.LastName, Email = customer.Email, PhoneNumber = customer.PhoneNumber, RegisteredAt = customer.RegisteredAt}, 200);
        }


        public  Task<PagedResponse<CustomerSummaryDto>> GetAllCustomers(CustomerQueryFilter filter)
        {
            return _CustomerRepository.GetAllCustomer(filter);
        }

        public async Task<ServiceResult> UpdateCustomer(int PersonId, UpdateCustomerDto dto)
        {
            var customer = await _CustomerRepository.GetCustomerByID(PersonId);

            if (customer == null)
            {
                return ServiceResult.Failure(404, "Customer not found.");
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
                    return ServiceResult.Failure(400, "Phone number is already taken.");
                }

                customer.PhoneNumber = dto.PhoneNumber;
            }

            if(await _personRepository.SaveChangeAsync() > 0)
            {
                return ServiceResult.Success(204);
            }

            return ServiceResult.Failure(500, "An error occurred while updating the customer.");    
        }


        public Task<bool> DeleteCustomerByID(int personID)
        {
            return _personRepository.SoftDeletePersonByIdAsync(personID);
        }

    }
}
