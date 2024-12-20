using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CRUDS_Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture fixture;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            // initial Data Source
            List<Country> countries = new List<Country>();
            List<Person> persons = new List<Person>();
            // Create option bulider
            var ApplicationDbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            // Create Mock DbContext
            DbContextMock<ApplicationDbContext> _dbContextMock = new DbContextMock<ApplicationDbContext>(ApplicationDbContextOptions);
            // Create Mock DbSet 
            _dbContextMock.CreateDbSetMock<Country>(temp => temp.Countries, countries);
            _dbContextMock.CreateDbSetMock<Person>(temp => temp.Persons, persons);
            // give mock DbContext to Service 
            _countriesService = new CountriesService(_dbContextMock.Object);
            _personService = new PersonService(_dbContextMock.Object, _countriesService);

            _outputHelper =  testOutputHelper;

            fixture = new Fixture();

           //  _countriesService = new CountriesService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
           // _personService  = new PersonService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options),_countriesService);

        }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
           
            var action =(async () =>
            {
                //Act
               await _personService.AddPerson(personAddRequest);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();  

        }
        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_NullNamePerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp=>temp.PersonName,null as string)
                .Create();

            var action = (async () =>
             {
                 //Act
                 await _personService.AddPerson(personAddRequest);
             });
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();


        }
        // Proper PersonAddRequest , return personResonsoe with generated newly PersonId
        [Fact]
        public async Task AddPerson_ProperDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"Sample@ex.com")
                .Create();
            // Act 
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> responseList = await _personService.GetAllPersons();
            //Assert
            personResponse.PersonId.Should().NotBe(Guid.Empty);
            responseList.Should().Contain(personResponse);
           //Assert.True(personResponse.PersonId != Guid.Empty);
          //Assert.Contains(personResponse, responseList);
           

        }
        #endregion

        #region GetPersonByPersonId
        // If PersonId is Null , then Return Null 
        [Fact]
        public async Task GetPersonByPersonId_NullID()
        {
            // Arrange 
            Guid? PersonId = null;
            // Act
           PersonResponse? person_Response_From_Get = await _personService.GetPersonByPersonId(PersonId);
            //Assert
            person_Response_From_Get.Should().BeNull();
          // Assert.Null(person_Response_From_Get);
        }
        // If PersonId Proper , then it will pass as  and return PersonResponse object
        [Fact]
        public async Task GetPersonByPersonId_ProperPersonId()
        {
            // Arrange 
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"sample@ex.com")
                .With(temp=>temp.CountryId,countryResponse.CountryId)
                .Create();
            PersonResponse person_response_form_add = await _personService.AddPerson(personAddRequest);
            // Act
            PersonResponse? person_Response_From_Get = await _personService.GetPersonByPersonId(person_response_form_add.PersonId);
            //Assert
            person_Response_From_Get.Should().Be(person_response_form_add);
           // Assert.Equal(person_response_form_add, person_Response_From_Get);
        }
        #endregion

        #region GetAllPerson
        // List should Empty By default
        [Fact]
        public async Task GetAllPerson_EmptyListByDefalut()
        {
            // Act
           List<PersonResponse> personResponses = await  _personService.GetAllPersons();
            //Assert
            personResponses.Should().BeEmpty();
           // Assert.Empty(personResponses);
        }
        // If Enter Some data , should return the same 
        [Fact]
        public async Task GetAllPerson_AddFewPerosns()
        {
            // Arrange 
            CountryAddRequest countryAddRequest1 = fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1); 
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"sample1@ex.com")
                .With(temp=>temp.CountryId,countryResponse1.CountryId)
                .Create();

            PersonAddRequest person_request_2 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample2@ex.com")
                .With(temp => temp.CountryId, countryResponse2.CountryId)
                .Create();

            PersonAddRequest person_request_3 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample3@ex.com")
                .With(temp => temp.CountryId, countryResponse1.CountryId)
                .Create();

            List<PersonAddRequest> person_request = new List<PersonAddRequest> {  person_request_1 , person_request_2, person_request_3 };

            List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

            foreach(PersonAddRequest personAddRequest in person_request)
            {
                PersonResponse personResponse = await  _personService.AddPerson(personAddRequest);
                person_responses_from_add.Add(personResponse);
            }
            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach(PersonResponse person in person_responses_from_add)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            // Act 
            List<PersonResponse> person_resopnse_form_get = await _personService.GetAllPersons();
            //Assert
            person_resopnse_form_get.Should().BeEquivalentTo(person_responses_from_add);
           //foreach(PersonResponse expected_Response in person_responses_from_add)
           // {
           //     Assert.Contains(expected_Response, person_resopnse_form_get);
           // }
            // print person_response_from_add
            _outputHelper.WriteLine("Actual : ");
            foreach (PersonResponse person in person_resopnse_form_get)
            {
                _outputHelper.WriteLine(person.ToString());
            }
        }
        #endregion

        #region GetFilterdPerson
        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilterdPerson_EmptySearchText()
        {
            // Arrange 
            CountryAddRequest countryAddRequest1 = fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = fixture.Build<PersonAddRequest>()
               .With(temp => temp.Email, "sample1@ex.com")
               .With(temp => temp.CountryId, countryResponse1.CountryId)
               .Create();

            PersonAddRequest person_request_2 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample2@ex.com")
                .With(temp => temp.CountryId, countryResponse2.CountryId)
                .Create();

            PersonAddRequest person_request_3 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample3@ex.com")
                .With(temp => temp.CountryId, countryResponse1.CountryId)
                .Create();

            List<PersonAddRequest> person_request = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in person_request)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                person_responses_from_add.Add(personResponse);
            }
            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_responses_from_add)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            // Act 
            List<PersonResponse> person_resopnse_form_search = await _personService.GetFilterdPersons(nameof(Person.PersonName),"");
            //Assert
            person_resopnse_form_search.Should().BeEquivalentTo(person_responses_from_add);
            //foreach (PersonResponse expected_Response in person_responses_from_add)
            //{
            //    Assert.Contains(expected_Response, person_resopnse_form_search);
            //}
            // print person_response_from_search
            _outputHelper.WriteLine("Actual : ");
            foreach (PersonResponse person in person_resopnse_form_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }
        }

        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilterdPerson_SearchByPersonName()
        {
            // Arrange 
            CountryAddRequest countryAddRequest1 = fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = fixture.Build<PersonAddRequest>()
               .With(temp=>temp.PersonName,"Maged")
               .With(temp => temp.Email, "sample1@ex.com")
               .With(temp => temp.CountryId, countryResponse1.CountryId)
               .Create();

            PersonAddRequest person_request_2 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "mary")
                .With(temp => temp.Email, "sample2@ex.com")
                .With(temp => temp.CountryId, countryResponse2.CountryId)
                .Create();

            PersonAddRequest person_request_3 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "sayed")
                .With(temp => temp.Email, "sample3@ex.com")
                .With(temp => temp.CountryId, countryResponse1.CountryId)
                .Create();

            List<PersonAddRequest> person_request = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in person_request)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                person_responses_from_add.Add(personResponse);
            }
            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_responses_from_add)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            // Act 
            List<PersonResponse> person_resopnse_form_search = await _personService.GetFilterdPersons(nameof(Person.PersonName), "ma");
            //Assert
            person_resopnse_form_search.Should().OnlyContain(temp=>temp.PersonName.Contains("ma",StringComparison.OrdinalIgnoreCase));
            //foreach (PersonResponse expected_Response in person_responses_from_add)
            //{
            //    if (expected_Response.PersonName != null)
            //    {
            //        if (expected_Response.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(expected_Response, person_resopnse_form_search);
            //        }
            //    }
            //}
            // print person_response_from_search
            _outputHelper.WriteLine("Actual : ");
            foreach (PersonResponse person in person_resopnse_form_search)
            {
                _outputHelper.WriteLine(person.ToString());
            }
        }
        #endregion

        #region GetSortedPersons
        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest countryAddRequest1 = fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = fixture.Build<PersonAddRequest>()
               .With(temp => temp.Email, "sample1@ex.com")
               .With(temp => temp.CountryId, countryResponse1.CountryId)
               .Create();

            PersonAddRequest person_request_2 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample2@ex.com")
                .With(temp => temp.CountryId, countryResponse2.CountryId)
                .Create();

            PersonAddRequest person_request_3 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample3@ex.com")
                .With(temp => temp.CountryId, countryResponse1.CountryId)
                .Create();

            List<PersonAddRequest> person_request = new List<PersonAddRequest> { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_responses_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in person_request)
            {
                PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
                person_responses_from_add.Add(personResponse);
            }

            // DescOrder maunally
            person_responses_from_add = person_responses_from_add.OrderByDescending(x => x.PersonName).ToList();

            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_responses_from_add)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            List<PersonResponse> all_persons = await _personService.GetAllPersons();

            // Act 
            List<PersonResponse> person_resopnse_form_sort = await _personService.GetSortedPersons(all_persons,nameof(Person.PersonName), SortOrderOptions.Desc);

           

            // print person_response_from_search
            _outputHelper.WriteLine("Actual : ");
            foreach (PersonResponse person in person_resopnse_form_sort)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            //Assert

           // person_resopnse_form_sort.Should().BeEquivalentTo(person_responses_from_add);

            //for (int i = 0; i < person_responses_from_add.Count; i++)
            //{
            //    Assert.Equal(person_responses_from_add[i], person_resopnse_form_sort[i]);
            //}
            person_resopnse_form_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
           
        }
        #endregion

        #region UpdatePerson
        // if Supply Null Object , Should Throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
           
            var action = (async () =>
            {
                // Act
               await  _personService.UpdatePerson(personUpdateRequest);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        // if supply personid invalid , should throw ArgumantException
        [Fact]
        public async Task UpdatePerson_InvalidPersonId()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = fixture.Build<PersonUpdateRequest>()
                .With(temp => temp.Email, "sample@ex.com")
                .Create();
               
           
            var action = (async () =>
            {
                // Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // if Supply PersonName Null , Should Throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();  
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@ex.com")
                .Create();
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            PersonUpdateRequest person_update_request_From_add = personResponse.ToPersonUpdateRequest();
            person_update_request_From_add.PersonName = null;

        
            var action = (async () =>
            {
                // Act
                await _personService.UpdatePerson(person_update_request_From_add);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
           
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_ProperPerson()
        {
            //Arrange
            CountryAddRequest country_add_request = fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@ex.com")
                .With(temp => temp.CountryId, country_response_from_add.CountryId)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_update.PersonId);

            //Assert
            person_response_from_update.Should().Be(person_response_from_get);
            //Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

        #region DeletePerson
        // if Supply invalid Id , should return false
        [Fact]
        public async Task DeletePerson_invalidPersonId()
        {
            // Act
            bool IsDeleted = await _personService.DeletePerson(Guid.NewGuid());
            //Assert
            IsDeleted.Should().BeFalse();
           // Assert.False(IsDeleted);
        }
        // if Supply valid Id , should return true
        [Fact]
        public async void DeletePerson_validPersonId()
        {
            //Arrange
            CountryAddRequest country_add_request = fixture.Create<CountryAddRequest>();   
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "sample@ex.com")
                .With(temp => temp.CountryId, country_response_from_add.CountryId)
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


            // Act
            bool IsDeleted = await _personService.DeletePerson(person_response_from_add.PersonId);
            //Assert
            IsDeleted.Should().BeTrue();
           // Assert.True(IsDeleted);
        }
        #endregion
    }
}
