﻿using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model For Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }
        [StringLength(20)]
        public string? CountryName { get; set; }

        public virtual ICollection<Person>? Persons { get; set; }
    }
}
