using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountryService
{
    private readonly List<Country> _countries;

    public CountriesService(bool initialize = true)
    {
        _countries = [];
        if (initialize)
        {
            // A3F84C4A-8CE2-4A15-B560-D4DFEB09B328
            // 0E241979-E826-47E9-837F-C0533E5F88FF
            // 4FFA4035-1DDF-4005-A72A-48FF4D47F7AB
            // 657D8B4A-FBA4-4B99-AD17-F738DD7D8F7F
            // D04E3EA8-E52E-480C-9ABF-15D2D97586B9
            // 9254EEF2-45B1-4F09-9A7D-423A9E582A8C
            _countries.AddRange(
                new Country() { CountryId = Guid.Parse("A3F84C4A-8CE2-4A15-B560-D4DFEB09B328"), Name = "Greece" },
                new Country() { CountryId = Guid.Parse("0E241979-E826-47E9-837F-C0533E5F88FF"), Name = "France" },
                new Country() { CountryId = Guid.Parse("4FFA4035-1DDF-4005-A72A-48FF4D47F7AB"), Name = "Germany" },
                new Country() { CountryId = Guid.Parse("657D8B4A-FBA4-4B99-AD17-F738DD7D8F7F"), Name = "England" },
                new Country() { CountryId = Guid.Parse("D04E3EA8-E52E-480C-9ABF-15D2D97586B9"), Name = "U.S.A" },
                new Country() { CountryId = Guid.Parse("9254EEF2-45B1-4F09-9A7D-423A9E582A8C"), Name = "Russia" });
            
            


        }
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
        if (_countries.Any(c => c.Name == countryAddRequest.CountryName))
            throw new ArgumentException("Given country already exists");

        // Convert countryAddRequest to type Country
        Country country = countryAddRequest.ToCountry();
        
        // Generate CountryId
        country.CountryId = Guid.NewGuid();
        
        // Add it to the list
        _countries.Add(country);

        // Return the created country as a CountryResponse
        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _countries.Select(c => c.ToCountryResponse()).ToList();
    }
    
    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        return countryId == null ? null 
            : _countries.FirstOrDefault(c => c.CountryId.Equals(countryId))
                ?.ToCountryResponse();
    }
}