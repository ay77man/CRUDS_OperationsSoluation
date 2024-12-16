using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Entities
{
    /// <summary>
    /// Domain Model For Person Entity
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }

        [StringLength(50)]
        public string? PersonName { get; set; }

        [StringLength(50)]
        public string? Email {  get; set; }
        public DateTime? DataOfBirth {  get; set; }

        [StringLength(10)]
        public string? Gender {  get; set; }

        [StringLength(50)]
        public Guid? CountryId {  get; set; }

        [StringLength(100)]
        public string? Address { get; set; }
        public bool ReceiveNewsLetters {  get; set; }

       //[Column( "TaxIdentificationNumber" , TypeName ="varchar(8)")]
        public string? TIN { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }
    }
}
