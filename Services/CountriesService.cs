using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
           _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if ((await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null))
                throw new ArgumentException("Country Name Duplicated !!!");

            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid(); 

            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();
              return countries.Select(temp=>temp.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? CountryId)
        {
          if(CountryId == null)
                return null;

          Country? country = await _countriesRepository.GetCountryByCountryId(CountryId.Value);

            if(country == null)
                return null;
            return country.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if ( await _countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);
                            countriesInserted++;
                        }
                    }
                }
            }

            return countriesInserted;
        }

    }
}
