using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;



namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countries;
        

        // Consturctor
        public PersonService(PersonsDbContext personsDbContext , ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countries = countriesService;   
        }
     
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            // if Object null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //Model validations
            ValidationHelper.ValidationModel(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();
          
        }

        public async Task<bool> DeletePerson(Guid? PersonId)
        {
            if (PersonId == null)
                return false;

            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(p=>p.PersonId == PersonId);

            if (matchingPerson == null)
                return false;

            _db.Persons.Remove(matchingPerson);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
           var persons = await _db.Persons.Include(p=>p.Country).ToListAsync();
             return persons.
                Select(temp=>temp.ToPersonResponse()).ToList();

        }

        public async Task<List<PersonResponse>> GetFilterdPersons(string? searchBy, string? searchString)
        {
           List<PersonResponse> all_persons = await GetAllPersons();  
           List<PersonResponse> matching_persons = all_persons;  
            if(string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
                return matching_persons;
            switch(searchBy)
            {
                case nameof(Person.PersonName):
                    matching_persons = all_persons.Where(temp=>
                    (!string.IsNullOrEmpty(temp.PersonName) ?
                    temp.PersonName.Contains(searchString,StringComparison.OrdinalIgnoreCase) : true )).ToList();
                    break;
                case nameof(Person.Email):
                    matching_persons = all_persons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Email) ?
                    temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.Address):
                    matching_persons = all_persons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Address) ?
                    temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.Gender):
                    matching_persons = all_persons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Gender) ?
                    temp.Gender.Equals(searchString,StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(Person.DataOfBirth):
                    matching_persons = all_persons.Where(temp =>
                    (temp.DataOfBirth != null) ?
                    temp.DataOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                default:
                    matching_persons = all_persons;
                    break;


            }
                return matching_persons;


        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? PersonId)
        {
            if (PersonId == null)
                return null;
            Person? person = await _db.Persons.Include(p=>p.Country).FirstOrDefaultAsync(p => p.PersonId == PersonId);
            if (person == null)
                return null;
          return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> all_persons, string? SortBy, SortOrderOptions sortOrder)
        {
           if(string.IsNullOrEmpty(SortBy))
                return  all_persons;
            List<PersonResponse> SortedPersons = (SortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.Age).ToList(),
                (nameof(PersonResponse.DataOfBirth), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.DataOfBirth).ToList(),
                (nameof(PersonResponse.DataOfBirth), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.DataOfBirth).ToList(),
                (nameof(PersonResponse.CountryName), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.CountryName), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Asc) => all_persons.OrderBy(p => p.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Desc) => all_persons.OrderByDescending(p => p.ReceiveNewsLetters).ToList(),

                _ => all_persons
            } ;
            return SortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            ValidationHelper.ValidationModel(personUpdateRequest);

            //get matching person object to update
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personUpdateRequest.PersonID);
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DataOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryId = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }
    }
}
