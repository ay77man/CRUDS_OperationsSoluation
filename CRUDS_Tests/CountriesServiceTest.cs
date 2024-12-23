using Xunit;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;

namespace CRUDS_Tests
{
    public class CountriesServiceTest
    {
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        private readonly ICountriesService _countriesService;

        private readonly IFixture fixture;

        // Constructor
        public CountriesServiceTest()
        {
            // mock the ICountriesRepository
           _countriesRepositoryMock = new Mock<ICountriesRepository>();
            // Mocked Object 
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);

            fixture = new Fixture();
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
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
        public async Task AddCountry_NullCountryName_ToBeArgumentException()
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
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? request1 = fixture.Create<CountryAddRequest>();

            Country country = request1.ToCountry(); 

            _countriesRepositoryMock.Setup(temp=>temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country);

           var action = (async () =>
            {
                // Act 
               await _countriesService.AddCountry(request1);              
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountry_ToBeSuccessful()
        {
            // Arrange
            CountryAddRequest? request1 = fixture.Create<CountryAddRequest>();

            Country country = request1.ToCountry();
            CountryResponse countryResponse_Expected = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            // Act 
            CountryResponse countryResponse_From_Add = await _countriesService.AddCountry(request1);
            countryResponse_Expected.CountryId = countryResponse_From_Add.CountryId;
            // Assert
            countryResponse_From_Add.CountryId.Should().NotBe(Guid.Empty);
            countryResponse_From_Add.Should().Be(countryResponse_Expected);
            //Assert.True(countryResponse.CountryId != Guid.Empty);
           
        }
        #endregion

        #region GetAllCountries
        //The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyListByDefalut()
        {
            //Arrange 
            List<Country> countries = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

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
            List<Country> countryAdds = new List<Country>
            {
                 fixture.Build<Country>()
                   .With(temp => temp.Persons, null as List<Person>).Create(),
                 fixture.Build<Country>()
                   .With(temp => temp.Persons, null as List<Person>).Create()
            };

            List<CountryResponse> countries_responses_Expected = countryAdds.Select(temp=>temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countryAdds);


            //Act          
            List<CountryResponse> Country_Response_From_GetAllCoutries = await _countriesService.GetAllCountries();
            // Assert
            Country_Response_From_GetAllCoutries.Should().BeEquivalentTo(countries_responses_Expected);
           
        }
        #endregion

        #region GetCountryByCountryId 
        // If CountryId = Null , Then Sould Return Null
        [Fact]
        public async Task GetCountryById_IdIsNull_ToBeNull()
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
            Country country = fixture.Build<Country>()
                .With(temp=>temp.Persons,null as List<Person>)
                .Create();

            CountryResponse ExpectedResponse = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp=>temp.GetCountryByCountryId(It.IsAny<Guid>()))
               .ReturnsAsync(country);

            // Act 
            CountryResponse? ActualResonse = await _countriesService.GetCountryByCountryId(ExpectedResponse.CountryId);
            //Assert
            ActualResonse.Should().Be(ExpectedResponse);
           // Assert.Equal(ExpectedResponse, ActualResonse);

        }
        #endregion


    }
}
