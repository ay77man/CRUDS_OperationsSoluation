using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDS_Tests
{
    public class PersonServiceTest
    {
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository personsRepository;

        private readonly IPersonService _personService;
       
        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture fixture;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            personsRepository = _personsRepositoryMock.Object;
            _personService = new PersonService(personsRepository);
          
            _outputHelper = testOutputHelper;

            fixture = new Fixture();

        }
        #region AddPerson
        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            var action = (async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

        }
        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_NullNamePerson_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
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
        public async Task AddPerson_ProperDetails_ToBeSucessful()
        {
            // Arrange
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "Sample@ex.com")
                .Create();

            Person person = personAddRequest.ToPerson();

            // if we supply argument from type of Person in PersonRepository.AddPerson , should Return the same person 
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse personResponse_Expected = person.ToPersonResponse();

            // Act 
            PersonResponse personResponse_from_Add = await _personService.AddPerson(personAddRequest);
            personResponse_Expected.PersonId = personResponse_from_Add.PersonId;

            //Assert
            // personResponse_from_Add.PersonId.Should().NotBe(Guid.Empty); // not required 
            personResponse_from_Add.Should().Be(personResponse_Expected);
            //Assert.True(personResponse.PersonId != Guid.Empty);

        }
        #endregion

        #region GetPersonByPersonId
        // If PersonId is Null , then Return Null 
        [Fact]
        public async Task GetPersonByPersonId_NullID_ToBeNull()
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
        public async Task GetPersonByPersonId_ProperPersonId_ToBeSecessful()
        {
            // Arrange 
            Person person = fixture.Build<Person>()
                .With(temp => temp.Email, "sample@ex.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponse_Expected = person.ToPersonResponse();
            // if we use PersonsRepository.GetPersonByPersonId , should return this person
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);


            // Act
            PersonResponse? person_Response_From_Get = await _personService.GetPersonByPersonId(person.PersonId);
            //Assert
            person_Response_From_Get.Should().Be(personResponse_Expected);
            // Assert.Equal(person_response_form_add, person_Response_From_Get);
        }
        #endregion

        #region GetAllPerson
        // List should Empty By default
        [Fact]
        public async Task GetAllPerson_ToBeEmptylist()
        {
            List<Person> persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            // Act
            List<PersonResponse> personResponses = await _personService.GetAllPersons();
            //Assert
            personResponses.Should().BeEmpty();
            // Assert.Empty(personResponses);
        }
        // If Enter Some data , should return the same 
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
                //Arrange
                List<Person> persons = new List<Person>() {
                     fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create()
                    };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //print person_response_list_from_add
            _outputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _outputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //print persons_list_from_get
            _outputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
               _outputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilterdPerson
        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilterdPerson_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                     fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create()
                    };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            _personsRepositoryMock.Setup(temp=>temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
                .ReturnsAsync(persons);
            // Act 
            List<PersonResponse> person_resopnse_form_search = await _personService.GetFilterdPersons(nameof(Person.PersonName),"");
            //Assert
            person_resopnse_form_search.Should().BeEquivalentTo(person_response_list_expected);
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
        public async Task GetFilterdPerson_SearchByPersonName_ToBeSuccessful()
        {
            // Arrange 
            
            List<Person> persons = new List<Person>() {
                     fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create()
                    };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
              .ReturnsAsync(persons);

            // Act 
            List<PersonResponse> person_resopnse_form_search = await _personService.GetFilterdPersons(nameof(Person.PersonName), "ma");

            //Assert
            person_resopnse_form_search.Should().BeEquivalentTo(person_response_list_expected);

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
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            // Arrange 
            List<Person> persons = new List<Person>() {
                     fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create(),

                    fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.Country, null as Country)
                    .Create()
                    };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            // print person_response_from_add
            _outputHelper.WriteLine("Expected : ");
            foreach (PersonResponse person in person_response_list_expected)
            {
                _outputHelper.WriteLine(person.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> all_persons = await _personService.GetAllPersons();

            // Act 
            List<PersonResponse> person_resopnse_form_sort = await _personService.GetSortedPersons(all_persons,nameof(Person.PersonName), SortOrderOptions.Desc);

           

            // print person_response_from_search
            _outputHelper.WriteLine("Actual : ");
            foreach (PersonResponse person in person_resopnse_form_sort)
            {
                _outputHelper.WriteLine(person.ToString());
            }        
            person_resopnse_form_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
           
        }
        #endregion

        #region UpdatePerson
        // if Supply Null Object , Should Throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
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
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            // Arrange  
            Person person = fixture.Build<Person>()
                .With(temp => temp.Email, "sample@ex.com")
                .With(temp=>temp.PersonName,null as string)
                .With(temp=>temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

             PersonResponse personResponse = person.ToPersonResponse();
             PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();

            var action = (async () =>
            {
                // Act
                await _personService.UpdatePerson(personUpdate);
            });
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
           
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_ProperPerson_ToBeSuccessful()
        {
            // Arrange  
            Person person = fixture.Build<Person>()
                .With(temp => temp.Email, "sample@ex.com")              
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "male")
                .Create();

            PersonResponse personResponse_Expected = person.ToPersonResponse();
            PersonUpdateRequest personUpdate = personResponse_Expected.ToPersonUpdateRequest();

            _personsRepositoryMock
              .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
              .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(personUpdate);


            //Assert
            person_response_from_update.Should().Be(personResponse_Expected);
            //Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

        #region DeletePerson
        // if Supply invalid Id , should return false
        [Fact]
        public async Task DeletePerson_invalidPersonId_ToBeFalse()
        {
           // Act
            bool IsDeleted = await _personService.DeletePerson(Guid.NewGuid());
            //Assert
            IsDeleted.Should().BeFalse();
           // Assert.False(IsDeleted);
        }
        // if Supply valid Id , should return true
        [Fact]
        public async void DeletePerson_validPersonId_ToBeSuccessful()
        {
            // Arrange  
            Person person = fixture.Build<Person>()
                .With(temp => temp.Email, "sample@ex.com")
                .With(temp => temp.Country, null as Country)
                .Create();

           // PersonResponse personResponse_Expected = person.ToPersonResponse();
           

            _personsRepositoryMock
              .Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
              .ReturnsAsync(person);

            // Act
            bool IsDeleted = await _personService.DeletePerson(person.PersonId);
            //Assert
            IsDeleted.Should().BeTrue();
           // Assert.True(IsDeleted);
        }
        #endregion
    }
}
