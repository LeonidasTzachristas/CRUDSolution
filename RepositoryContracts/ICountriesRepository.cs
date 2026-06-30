using Entities;

namespace RepositoryContracts;

/// <summary>
/// Represents data access logic for managing Country Entity
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// Adds a new country object to the data store
    /// </summary>
    /// <param name="country">Country object to add</param>
    /// <returns>Returns the country object after adding it to the data store</returns>
    Task<Country> AddCountry(Country country);

    /// <summary>
    /// Returns all the countries in the data store
    /// </summary>
    /// <returns>The list of countries from the table</returns>
    Task<List<Country>> GetAllCountries();

    /// <summary>
    /// Returns a country object based on the given country id
    /// </summary>
    /// <param name="countryId">The country id to search on</param>
    /// <returns>Returns the country with the specified country id or null if not found</returns>
    Task<Country?> GetCountryByCountryId(Guid countryId);

    /// <summary>
    /// Returns the country object with the given name
    /// </summary>
    /// <param name="countryName">The name of the country to search</param>
    /// <returns>Returns the country with the specified name or <b>null</b> if not found</returns>
    Task<Country?> GetCountryByCountryName(string countryName);
}