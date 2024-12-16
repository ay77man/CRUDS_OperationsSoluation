using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class Acts As Retrun type for most methods of PersonService
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DataOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Override the Equals Method to compare the values of object not reference 
        /// </summary>
        /// <param name="obj">the another object</param>
        /// <returns>ture of false based on values</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is PersonResponse)) return false;
            PersonResponse person = (PersonResponse)obj;
            return PersonId == person.PersonId && PersonName == person.PersonName && DataOfBirth == person.DataOfBirth && CountryId == person.CountryId && Gender == person.Gender && ReceiveNewsLetters == person.ReceiveNewsLetters && Address == person.Address && Email == person.Email;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $" PersonId : {PersonId} , PersonName : {PersonName} , Email : {Email} , Gender {Gender} , DataOfBirth : {DataOfBirth?.ToString("dd MMM yyyy ")} , Age{Age} , Address {Address} , CountryId : {CountryId} , Country : {CountryName} , ReceiveNewsLetters : {ReceiveNewsLetters}  ";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest
            {
                PersonID = PersonId,
                PersonName = PersonName,
                DateOfBirth = DataOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                Address = Address,
                Email = Email,
                CountryID = CountryId,
                ReceiveNewsLetters = ReceiveNewsLetters

            };
        } 
    }
    /// <summary>
    /// Class to convert person object to PersonResopnse object .
    /// </summary>
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonId = person.PersonId,
                Address = person.Address,
                Email = person.Email,
                DataOfBirth = person.DataOfBirth,
                Gender = person.Gender,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                CountryId = person.CountryId,
                PersonName = person.PersonName,
                Age = (person.DataOfBirth != null) ? Math.Round((DateTime.Now - person.DataOfBirth.Value).TotalDays / 365.25) : null,
                CountryName = person.Country?.CountryName

        };
        }
    }
    
}
