using StudyHubAPI.Repositories;
using StudyHubAPI.Models.DTOs.Country;
using StudyHubAPI.Utils;


namespace StudyHubAPI.Services
{
    public class CountryService
    {
        private readonly CountryRepository _countryRepository;

        public CountryService(CountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }


        public async Task<ServiceResult<List<CountrySummaryDto>>> GetAllCountries()
        {
            var countries = await _countryRepository.GetAllCountries();

            if(countries.Count == 0)
            {
                return ServiceResult<List<CountrySummaryDto>>.Failure(ResultType.NotFound, "No countries found.");
            }
            return ServiceResult<List<CountrySummaryDto>>.Success(countries);
        }

    }
}
