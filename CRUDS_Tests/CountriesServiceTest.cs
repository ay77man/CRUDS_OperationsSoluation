using Xunit;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;


namespace CRUDS_Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        // Constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>( async()=>
            {
                // Act 
                await _countriesService.AddCountry(request);
            });
        }
        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = null};
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act 
               await _countriesService.AddCountry(request);
            });
        }
        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest { CountryName = "USA"};
            CountryAddRequest? request2 = new CountryAddRequest { CountryName = "USA"};
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act 
               await _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
            });
        }
        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountry()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Egypt"};
            
            // Act 
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> AllCountries = await _countriesService.GetAllCountries();

            // Assert
            Assert.True(countryResponse.CountryId != Guid.Empty);
            Assert.Contains(countryResponse, AllCountries);
        }
        #endregion

        #region GetAllCountries
        //The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyListByDefalut()
        {
             // Act 
             List<CountryResponse> acutal_country_response_list = await _countriesService.GetAllCountries();
             // Assert
            Assert.Empty(acutal_country_response_list);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> countryAdds = new List<CountryAddRequest>
            {
                new CountryAddRequest { CountryName = "UK" },
                new CountryAddRequest { CountryName = "Qatar" }
            };
            //Act
              // this is Expected Data
            List<CountryResponse> Country_Response_From_Add_Request = new List<CountryResponse>();
            foreach(CountryAddRequest countryAdd in countryAdds)
            {
               CountryResponse countryResponse  = await _countriesService.AddCountry(countryAdd);
               Country_Response_From_Add_Request.Add(countryResponse);
            }
              // Acutal Data
            List<CountryResponse> Country_Response_From_GetAllCoutries = await _countriesService.GetAllCountries();
            // Assert
            foreach(CountryResponse Expected in Country_Response_From_Add_Request)
            {
                Assert.Contains(Expected, Country_Response_From_GetAllCoutries);
            }
        }
        #endregion

        #region GetCountryByCountryId 
        // If CountryId = Null , Then Sould Throw Null
        [Fact]
        public async Task GetCountryById_IdIsNull()
        {
            // Arrange
            Guid? CountryId = null;
            // Act 
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(CountryId);
            //Assert
            Assert.Null(countryResponse);   

        }
       
        // If Proper CountryId , Then Return the Matching CountryResponse Object.
        [Fact]
        public async Task GetCountryById_ProperCountryId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "Irland" };
            CountryResponse ExpectedResponse = await _countriesService.AddCountry(countryAddRequest);
            // Act 
            CountryResponse? ActualResonse = await _countriesService.GetCountryByCountryId(ExpectedResponse.CountryId);
            //Assert
            Assert.Equal(ExpectedResponse, ActualResonse);

        }
        #endregion


    }
}
