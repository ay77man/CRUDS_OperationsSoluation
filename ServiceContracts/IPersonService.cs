using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts
{
    /// <summary>
    /// Repersents Bussines Logic for manipulating Person Entity
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// For Adding New Person In Data Source
        /// </summary>
        /// <param name="personAddRequest">Person to add </param>
        /// <returns> generated personResponse including newly PersonId </returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
        /// <summary>
        /// Return All Persons form data source
        /// </summary>
        /// <returns>List of object from PersonResponse</returns>
        Task<List<PersonResponse>> GetAllPersons();
        /// <summary>
        /// Get Person based on PersonID
        /// </summary>
        /// <param name="PersonId"> PersonId to search</param>
        /// <returns>mathching person object as PersonResponse</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? PersonId);
        /// <summary>
        /// to get FilterdPersons based on searchFiled and searchString 
        /// </summary>
        /// <param name="searchBy">filed search with it </param>
        /// <param name="searchString">some string in this filed</param>
        /// <returns>Matching persons based on filed searched by it and some string </returns>
        Task<List<PersonResponse>> GetFilterdPersons(string? searchBy, string? searchString);
        /// <summary>
        /// To Sort List Of Persons ASC Or DESC
        /// </summary>
        /// <param name="all_persons"> all person that have </param>
        /// <param name="SortBy">filed that sort by it </param>
        /// <param name="sortOrder">DESC or ASC</param>
        /// <returns>Return all Sorted Persons</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> all_persons,string? SortBy , SortOrderOptions sortOrder);
        /// <summary>
        /// To Update in details of  Person object
        /// </summary>
        /// <param name="personUpdateRequest">Contain details of person that be updated</param>
        /// <returns>Return Updated Person</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        Task<bool> DeletePerson(Guid? PersonId);

        /// <summary>
        /// Returns persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV data</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns>Returns the memory stream with Excel data of persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
