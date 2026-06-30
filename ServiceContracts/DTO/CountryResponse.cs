using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO record used as return type for most of CountriesService methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? Name { get; set; }

    // Override this to use it on the Assert.Contains() of the unit test
    // Or preferably use record type in the class ???
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        
        if (obj.GetType() != typeof(CountryResponse))
            return false;

        var compareCountry = (CountryResponse)obj;// as CountryResponse;
        
        return CountryId == compareCountry.CountryId 
               && Name == compareCountry.Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{ CountryId = {CountryId}, Name = {Name} }}\n";
    }
}

public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            Name = country.Name
        };
    }
}
