using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Diagnostics.Metrics;

namespace Repository
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
             return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid? countryId)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryId == countryId);          
        }

        public async Task<Country?> GetCountryByCountryName(string CountryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryName == CountryName);
        }
    }
}
