﻿using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents Bussiness Logic For Country Model
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// For Adding A new Conuntry in the Data Source 
        /// </summary>
        /// <param name="countryAddRequest">Country Object to Add</param>
        /// <returns>Country Object After Adding (Include Newly generated CountryID)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        Task<List<CountryResponse>> GetAllCountries();
        Task<CountryResponse?> GetCountryByCountryId(Guid? CountryId);
        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
