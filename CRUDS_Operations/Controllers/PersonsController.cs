using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;


namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsController(IPersonService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;   
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
            string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.Asc)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
              {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DataOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryId), "Country" },
                { nameof(PersonResponse.Address), "Address" }
              };
            List<PersonResponse> persons = await _personsService.GetFilterdPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons); //Views/Persons/Index.cshtml
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            ViewBag.CountryList = countryResponses
                .Select(t => new SelectListItem { Text = t.CountryName ,Value = t.CountryId.ToString() });
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
                ViewBag.CountryList = countryResponses
                    .Select(t => new SelectListItem { Text = t.CountryName, Value = t.CountryId.ToString() });
                
                ViewBag.Errors = ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                return View();
            }
            await _personsService.AddPerson(personAddRequest);
            return RedirectToAction("index","Persons");
        }

        [HttpGet]
        [Route("[action]/{PersonId}")] 
        public async Task<IActionResult> Edit(Guid PersonId)
        { 
            PersonResponse? person_from_get = await _personsService.GetPersonByPersonId(PersonId);
            if(person_from_get == null)
            {
                return RedirectToAction("index");
            }

            PersonUpdateRequest personUpdateRequest = person_from_get.ToPersonUpdateRequest();
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.CountryList = countries.Select(t=>new SelectListItem { Text = t.CountryName , Value = t.CountryId.ToString()});
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{PersonId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? persons_response = await _personsService.GetPersonByPersonId(personUpdateRequest.PersonID);
            if(persons_response == null)
            {
                return RedirectToAction("index");
            }
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.CountryList = countries.Select(t => new SelectListItem { Text = t.CountryName, Value = t.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                return View(personUpdateRequest);
            }
             PersonResponse personResponse = await _personsService.UpdatePerson(personUpdateRequest);
            
            return RedirectToAction("index", "persons");
        }

        [HttpGet]
        [Route("[action]/{PersonId}")]
        public async Task<IActionResult> Delete(Guid PersonId)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonId(PersonId);
            if (person == null)
            {
                return RedirectToAction("index");
            }

            return View(person);
        }

        [HttpPost]
        [Route("[action]/{PersonId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonId(personUpdateRequest.PersonID);
            if (person == null)
            {
                return RedirectToAction("index");
            }
           
             bool IsDeleted = await  _personsService.DeletePerson(person.PersonId);
            return RedirectToAction("index");
        }
    }
}