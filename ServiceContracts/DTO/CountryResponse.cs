

using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class For Return Country Object As Retrun Type
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
           if(obj== null) return false;

           if(obj.GetType() != typeof(CountryResponse)) return false;

           CountryResponse other = (CountryResponse)obj;
           return CountryId == other.CountryId && CountryName == other.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// For Convert Country Object To Country Response 
    /// </summary>
    public static class CountryResponseExtionstions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }

}
