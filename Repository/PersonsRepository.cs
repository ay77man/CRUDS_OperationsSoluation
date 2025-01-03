﻿using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext applicationDbContext,ILogger<PersonsRepository> logger)
        {
            _db = applicationDbContext;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<Person> DeletePersonByPersonId(Guid? PersonId)
        {
            Person? person =  await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == PersonId);
            if (person == null)
            {
                return person;
            }
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            // logging
            _logger.LogInformation("GetAllPersons of persons Repository");

          return  await _db.Persons
                .Include(temp=>temp.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            // logging
            _logger.LogInformation("GetFilteredPersons of persons Repository");

            return await _db.Persons
                .Include(temp => temp.Country)
                .Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid? PersonId)
        {
            return await _db.Persons
                 .Include(temp => temp.Country)
                 .FirstOrDefaultAsync(temp => temp.PersonId == PersonId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
          Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (matchingPerson == null)
            
                return person;
            

            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.Gender = person.Gender;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
            matchingPerson.Address = person.Address;
            matchingPerson.DataOfBirth = person.DataOfBirth;
            matchingPerson.CountryId = person.CountryId;

            await _db.SaveChangesAsync();

            return matchingPerson;
        }
    }
}
