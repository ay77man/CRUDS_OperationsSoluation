using Entities;
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
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService  = new PersonService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options),_countriesService);
            _outputHelper =  testOutputHelper;
        }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
               await _personService.AddPerson(personAddRequest);
            });

        }
        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_NullNamePerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest { PersonName = null};
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            });

        }
        // Proper PersonAddRequest , return personResonsoe with generated newly PersonId
        [Fact]
        public async Task AddPerson_ProperDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest { PersonName = "Name" , Address = "Alex" , CountryId = Guid.NewGuid(),Gender = GenderOptions.male, DataOfBirth = DateTime.Parse("2000-01-01"),Email = "ayman@example.com",ReceiveNewsLetters = true };
            // Act 
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> responseList = await _personService.GetAllPersons();
            //Assert
            Assert.True(personResponse.PersonId != Guid.Empty);
            Assert.Contains(personResponse, responseList);
           

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
            Assert.Null(person_Response_From_Get);
        }
        // If PersonId Proper , then it will pass as  and return PersonResponse object
        [Fact]
        public async Task GetPersonByPersonId_ProperPersonId()
        {
            // Arrange 
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "Canda" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryId = countryResponse.CountryId, DataOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.male, ReceiveNewsLetters = false };
            PersonResponse person_response_form_add = await _personService.AddPerson(personAddRequest);
            // Act
            PersonResponse? person_Response_From_Get = await _personService.GetPersonByPersonId(person_response_form_add.PersonId);
            //Assert
            Assert.Equal(person_response_form_add, person_Response_From_Get);
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
            Assert.Empty(personResponses);
        }
        // If Enter Some data , should return the same 
        [Fact]
        public async Task GetAllPerson_AddFewPerosns()
        {
            // Arrange 
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName  = "USA"};
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName  = "Egypt"};

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1); 
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.male, Address = "address of smith", CountryId = countryResponse1.CountryId, DataOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.female, Address = "address of mary", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.male, Address = "address of rahman", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

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
           foreach(PersonResponse expected_Response in person_responses_from_add)
            {
                Assert.Contains(expected_Response, person_resopnse_form_get);
            }
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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Egypt" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.male, Address = "address of smith", CountryId = countryResponse1.CountryId, DataOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.female, Address = "address of mary", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.male, Address = "address of rahman", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

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
            foreach (PersonResponse expected_Response in person_responses_from_add)
            {
                Assert.Contains(expected_Response, person_resopnse_form_search);
            }
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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Egypt" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.male, Address = "address of smith", CountryId = countryResponse1.CountryId, DataOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.female, Address = "address of mary", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.male, Address = "address of rahman", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

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
            foreach (PersonResponse expected_Response in person_responses_from_add)
            {
                if (expected_Response.PersonName != null)
                {
                    if (expected_Response.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(expected_Response, person_resopnse_form_search);
                    }
                }
            }
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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Egypt" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.male, Address = "address of smith", CountryId = countryResponse1.CountryId, DataOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.female, Address = "address of mary", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.male, Address = "address of rahman", CountryId = countryResponse2.CountryId, DataOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

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
            for (int i = 0; i < person_responses_from_add.Count; i++)
            {
                Assert.Equal(person_responses_from_add[i], person_resopnse_form_sort[i]);
            }
           
        }
        #endregion

        #region UpdatePerson
        // if Supply Null Object , Should Throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            //Assert
             await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
               await  _personService.UpdatePerson(personUpdateRequest);
            });
        }
        // if supply personid invalid , should throw ArgumantException
        [Fact]
        public async Task UpdatePerson_InvalidPersonId()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest { PersonID = Guid.NewGuid() };
            //Assert
           await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }
        // if Supply PersonName Null , Should Throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "UK" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest { PersonName = "Ayman", Address = "sharkia", Email = "Ayman@s.com", Gender = GenderOptions.male, CountryId = countryResponse.CountryId, DataOfBirth = DateTime.Parse("2000 02 01"), ReceiveNewsLetters = true };
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            PersonUpdateRequest person_update_request_From_add = personResponse.ToPersonUpdateRequest();
            person_update_request_From_add.PersonName = null;

            //Assert
             await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _personService.UpdatePerson(person_update_request_From_add);
            });
           
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_ProperPerson()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryId = country_response_from_add.CountryId, Address = "Abc road", DataOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.male, ReceiveNewsLetters = true };

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_update.PersonId);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
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
            Assert.False(IsDeleted);
        }
        // if Supply valid Id , should return true
        [Fact]
        public async void DeletePerson_validPersonId()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryId = country_response_from_add.CountryId, DataOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.male, ReceiveNewsLetters = true };

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


            // Act
            bool IsDeleted = await _personService.DeletePerson(person_response_from_add.PersonId);
            //Assert
            Assert.True(IsDeleted);
        }
        #endregion
    }
}
