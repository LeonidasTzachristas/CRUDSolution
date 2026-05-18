using Entities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountryService
{
    private readonly PersonsDbContext _dbContext;

    public CountriesService(PersonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Check if countryAddRequest is null
        // Validate all properties of countryAddRequest
        // Convert countryAddRequest to type Country
        // Generate a new CountryId
        // Add to the List<Country> or Database
        // Return CountryResponse object with generated Id

        // Validation: countryAddRequest not null
        // if (countryAddRequest is null)
        // {
        //     throw new ArgumentNullException(nameof(countryAddRequest));
        // }
        ArgumentNullException.ThrowIfNull(countryAddRequest);
        
        // Validation: countryAddRequest.Name not null
        if (countryAddRequest.CountryName is null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        
        // Validation: Not duplicate country
        if (_dbContext.Countries.Any(c => c.Name == countryAddRequest.CountryName))
            throw new ArgumentException("Given country already exists");

        // Convert countryAddRequest to type Country
        Country country = countryAddRequest.ToCountry();
        
        // Generate CountryId
        country.CountryId = Guid.NewGuid();
        
        // Add it to the list
        _dbContext.Countries.Add(country);
        _dbContext.SaveChanges();

        // Return the created country as a CountryResponse
        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _dbContext.Countries.Select(c => c.ToCountryResponse()).ToList();
    }
    
    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        return countryId == null ? null 
            : _dbContext.Countries
                .FirstOrDefault(c => c.CountryId.Equals(countryId))
                ?.ToCountryResponse();
    }
}