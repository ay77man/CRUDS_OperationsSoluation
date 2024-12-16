using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService( PersonsDbContext personsDbContext)
        {
           _db = personsDbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
                throw new ArgumentException("Country Name Duplicated !!!");

            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid(); 

            _db.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries
                .Select(temp=>temp.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? CountryId)
        {
          if(CountryId == null)
                return null;
          Country? country = await _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryId == CountryId);
            if(country == null)
                return null;
            return country.ToCountryResponse();
        }
    }
}
