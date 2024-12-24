using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDS_Tests
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly IFixture fixture;

       public PersonsControllerTest()
        {
            fixture = new Fixture();

            _personServiceMock = new Mock<IPersonService>();
            _countriesServiceMock = new Mock<ICountriesService>();

            _personService = _personServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
        }

        #region Index 

        [Fact]
        public async Task Index_ToBeViewWithPersonResponseList()
        {
            // Arrange 
            List<PersonResponse> personResponses =  fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personService, _countriesService);

            _personServiceMock.Setup(temp=>temp.GetFilterdPersons(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(personResponses);
            _personServiceMock.Setup(temp=>temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>(),It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(personResponses);
           
            // Act
            IActionResult result =  await  personsController.Index(fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<SortOrderOptions>());

            // Assert

           ViewResult viewResult = Assert.IsType<ViewResult>(result);
           viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
           viewResult.ViewData.Model.Should().Be(personResponses);

        }

        #endregion

        #region Create

        [Fact]
        public async void Create_IfModelErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = fixture.Create<PersonAddRequest>();

            PersonResponse person_response = fixture.Create<PersonResponse>();

            List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

            _personServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);


            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

            viewResult.ViewData.Model.Should().Be(person_add_request);
        }


        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = fixture.Create<PersonAddRequest>();

            PersonResponse person_response = fixture.Create<PersonResponse>();

            List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

            _personServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);


            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion

        #region Edit
        [Fact]
        public async Task Edit_IfNullPersonseResponse()
        {
            //Arrange
            PersonUpdateRequest person_update_request = fixture.Create<PersonUpdateRequest>();
            PersonResponse? person_response = null;

            _personServiceMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);


            //Act
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Edit_IfModelErrorPersonseResponse()
        {
            //Arrange
            PersonUpdateRequest person_update_request = fixture.Create<PersonUpdateRequest>();
            List<CountryResponse> countryResponses = fixture.Build<List<CountryResponse>>().Create();
            PersonResponse person_response = fixture.Build<PersonResponse>().Create();


            _personServiceMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person_response);

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countryResponses);

            PersonsController personsController = new PersonsController(_personService, _countriesService);



            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
            viewResult.ViewData.Model.Should().Be(person_update_request);
        }

        [Fact]
        public async Task Edit_IfNoModelErrorPersonseResponse()
        {
            //Arrange
            PersonUpdateRequest person_update_request = fixture.Create<PersonUpdateRequest>();
            List<CountryResponse> countryResponses = fixture.Build<List<CountryResponse>>().Create();
            PersonResponse person_response = fixture.Build<PersonResponse>().Create();


            _personServiceMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person_response);

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countryResponses);

            _personServiceMock
             .Setup(temp => temp.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
             .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personService, _countriesService);



            //Act
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            RedirectToActionResult redirectToAction = Assert.IsType<RedirectToActionResult>(result);

            redirectToAction.ActionName.Should().Be("Index");
        }

        #endregion


    }
}
