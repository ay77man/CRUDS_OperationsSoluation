using Xunit;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDS_Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture fixture;

        // Constructor
        public CountriesServiceTest()
        {
            // initial Data Source
            List<Country> countries = new List<Country>();
            // Create option bulider
            var ApplicationDbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            // Create Mock DbContext
            DbContextMock<ApplicationDbContext> _dbContextMock = new DbContextMock<ApplicationDbContext>(ApplicationDbContextOptions);
            // Create Mock DbSet 
            _dbContextMock.CreateDbSetMock<Country>(temp => temp.Countries,countries);
            // give mock DbContext to Service 
            _countriesService = new CountriesService(_dbContextMock.Object);
            fixture = new Fixture();
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;


            // Act 
            var action = async () =>
            {
                await _countriesService.AddCountry(request);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

        }
        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            // Arrange
            CountryAddRequest? request = fixture.Build<CountryAddRequest>()
                .With(temp=>temp.CountryName , null as string)
            .Create();
         
            // Act 
            var action = (async () =>
            {
               await _countriesService.AddCountry(request);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = fixture.Create<CountryAddRequest>();
            CountryAddRequest? request2 = fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, request1.CountryName)
                .Create();         
           var action = (async () =>
            {
                // Act 
               await _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountry()
        {
            // Arrange
            CountryAddRequest? request = fixture.Create<CountryAddRequest>();
            
            // Act 
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> AllCountries = await _countriesService.GetAllCountries();

            // Assert
            countryResponse.CountryId.Should().NotBe(Guid.Empty);
            AllCountries.Should().Contain(countryResponse);
            //Assert.True(countryResponse.CountryId != Guid.Empty);
            //Assert.Contains(countryResponse, AllCountries);
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
             acutal_country_response_list.Should().BeEmpty();
            // Assert.Empty(acutal_country_response_list);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> countryAdds = new List<CountryAddRequest>
            {
                fixture.Create<CountryAddRequest>(),
                fixture.Create<CountryAddRequest>(),
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
            Country_Response_From_GetAllCoutries.Should().BeEquivalentTo(Country_Response_From_Add_Request);
            //foreach(CountryResponse Expected in Country_Response_From_Add_Request)
            //{
            //    Assert.Contains(Expected, Country_Response_From_GetAllCoutries);
            //}
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
            countryResponse.Should().BeNull();
           // Assert.Null(countryResponse);   

        }
       
        // If Proper CountryId , Then Return the Matching CountryResponse Object.
        [Fact]
        public async Task GetCountryById_ProperCountryId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();  
            CountryResponse ExpectedResponse = await _countriesService.AddCountry(countryAddRequest);
            // Act 
            CountryResponse? ActualResonse = await _countriesService.GetCountryByCountryId(ExpectedResponse.CountryId);
            //Assert
            ActualResonse.Should().Be(ExpectedResponse);
           // Assert.Equal(ExpectedResponse, ActualResonse);

        }
        #endregion


    }
}
