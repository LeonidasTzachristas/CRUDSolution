using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
public interface ICountryService
{
    
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Returns the country object after adding it (including the newly generated country ID)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Gets the entire List of countries
    /// </summary>
    /// <returns>Returns the List of CountryResponse</returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Returns a country object based on the given country id
    /// </summary>
    /// <param name="countryId">The Guid of the country to search</param>
    /// <returns>The country with the specified Id as CountryResponse</returns>
    Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);

    /// <summary>
    /// Uploads countries from excel file into database
    /// </summary>
    /// <param name="formFile">Excel file with list of countries</param>
    /// <returns>Returns the number of countries added</returns>
    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
}