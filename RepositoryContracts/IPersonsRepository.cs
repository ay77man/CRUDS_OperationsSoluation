using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represent Repository for interact with data store 
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// To Add Person
        /// </summary>
        /// <param name="person">Person that you add</param>
        /// <returns>person after added</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Get Person with it's Id
        /// </summary>
        /// <param name="PersonId">Id of peron</param>
        /// <returns>retrun the person based on id </returns>
        Task<Person?> GetPersonByPersonId(Guid? PersonId);

        /// <summary>
        /// all Persons in data store
        /// </summary>
        /// <returns>return all Persons</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// To Update Person Detalis
        /// </summary>
        /// <param name="person">person object</param>
        /// <returns>return perosn after updated</returns>
        Task<Person> UpdatePerson(Person person);

        /// <summary>
        /// To Delete Person From data base
        /// </summary>
        /// <param name="PersonId">Person id for delete</param>
        /// <returns>return the person after deleted</returns>
        Task<Person> DeletePersonByPersonId(Guid? PersonId);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

    }
}
