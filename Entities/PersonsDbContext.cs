
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Entities
{
    public class PersonsDbContext : DbContext
    {

        public PersonsDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");
           

            // Seed Data of County 
            string countryJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countryJson);

            foreach(Country country in countries)                
                modelBuilder.Entity<Country>().HasData(country);

            // Seed Data of Person 
            string personJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);

            // Fluent API 

            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("abc12345");

            ////Add index For Faster Search
            //modelBuilder.Entity<Person>().
            //   HasIndex(temp=>temp.TIN).IsUnique();

            //Add Check Constraint 
            modelBuilder.Entity<Person>()
         .HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            // Relations Between Person & Country 
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne(p => p.Country)
            //    .WithMany(c => c.Persons)
            //    .HasForeignKey(p => p.CountryId);
            //});
            


        }
    }
}
