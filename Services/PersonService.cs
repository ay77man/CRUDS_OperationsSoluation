using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
using System.Formats.Asn1;
using System.Globalization;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonService> _logger;
        // Consturctor
        public PersonService(IPersonsRepository personsRepository,ILogger<PersonService> logger )
        {
            _personsRepository = personsRepository;
            _logger = logger;
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

            await _personsRepository.AddPerson(person);

            return person.ToPersonResponse();
          
        }

        public async Task<bool> DeletePerson(Guid? PersonId)
        {
            if (PersonId == null)
                return false;

           Person person = await _personsRepository.DeletePersonByPersonId(PersonId);

            return person != null;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            // Logging
            _logger.LogInformation("GetAllPersons of PersonService ");

            var persons = await _personsRepository.GetAllPersons();
             return persons.
                Select(temp=>temp.ToPersonResponse()).ToList();

        }

        public async Task<List<PersonResponse>> GetFilterdPersons(string? searchBy, string? searchString)
        {
            // Logging
            _logger.LogInformation("GetFilterdPersons of PersonService ");
            List<Person> persons = searchBy switch

            {
                nameof(Person.PersonName) =>
                  await _personsRepository.GetFilteredPersons(
                      temp => temp.PersonName.Contains(searchString)),

                nameof(Person.Email) =>
                   await _personsRepository.GetFilteredPersons(
                       temp => temp.Email.Contains(searchString)),

                nameof(Person.Address) =>
                    await _personsRepository.GetFilteredPersons(
                        temp => temp.Address.Contains(searchString)),

                nameof(Person.Gender) =>
                    await _personsRepository.GetFilteredPersons(
                        temp => temp.Gender.Equals(searchString)),

                nameof(PersonResponse.DataOfBirth) =>
                  await _personsRepository.GetFilteredPersons(temp =>
                    temp.DataOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()


            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();


        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? PersonId)
        {
            if (PersonId == null)
                return null;
            Person? person = await _personsRepository.GetPersonByPersonId(PersonId);
            if (person == null)
                return null;
          return person.ToPersonResponse();
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

            csvWriter.WriteHeader<PersonResponse>(); //PersonID,PersonName,...
            csvWriter.NextRecord();

             List<PersonResponse> persons = await GetAllPersons();

            await csvWriter.WriteRecordsAsync(persons);
            //1,abc,....

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if (person.DataOfBirth.HasValue)
                        workSheet.Cells[row, 3].Value = person.DataOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.CountryName;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> all_persons, string? SortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(SortBy))
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
                throw new ArgumentNullException(nameof(Country));

            //validation
            ValidationHelper.ValidationModel(personUpdateRequest);

            //get matching person object to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonID);
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

            await _personsRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();
        }
    }
}
