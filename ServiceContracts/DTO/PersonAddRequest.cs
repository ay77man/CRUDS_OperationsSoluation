using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class For Adding A New Person to Data Source 
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name Can't be Empty")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email Can't be Empty")]
        [EmailAddress(ErrorMessage ="Enter a valid Mail ")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public DateTime? DataOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person { PersonName = PersonName , Address = Address , Email = Email, CountryId = CountryId,DataOfBirth = DataOfBirth , Gender = Gender.ToString(),ReceiveNewsLetters = ReceiveNewsLetters};
        }
    }
}
