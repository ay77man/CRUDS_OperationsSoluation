using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Repository to Interact with Data Store 
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// To Add Country
        /// </summary>
        /// <param name="country">Country that you add</param>
        /// <returns>country after added</returns>
       Task<Country> AddCountry(Country country);

        /// <summary>
        /// Get Country with it's Id
        /// </summary>
        /// <param name="CountryId">Id of country</param>
        /// <returns>retrun the country based on id </returns>
       Task<Country?> GetCountryByCountryId(Guid? countryId);

        /// <summary>
        /// all countries in data store
        /// </summary>
        /// <returns>return all countries</returns>
       Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Get Country with it's Name
        /// </summary>
        /// <param name="CountryId">Name of country</param>
        /// <returns>retrun the country based on Name </returns>
        Task<Country?> GetCountryByCountryName(string CountryName);

    }
}
