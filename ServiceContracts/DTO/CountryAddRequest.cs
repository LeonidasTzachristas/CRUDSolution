using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO class for adding a new country
/// </summary>
public record CountryAddRequest
{
    public string? CountryName { get; init; }

    /// <summary>
    /// Creates a new Country same as the CountryAddRequest
    /// </summary>
    /// <returns>Returns the created <b>Country</b> object</returns>
    public Country ToCountry()
    {
        return new Country() { Name = CountryName };
    }
}