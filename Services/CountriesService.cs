using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountryService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }
    
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {

        ArgumentNullException.ThrowIfNull(countryAddRequest);
        
        // Validation: countryAddRequest.Name not null
        if (countryAddRequest.CountryName is null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        
        // Validation: Not duplicate country
        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) is not null)
            throw new ArgumentException("Given country already exists");

        Country country = countryAddRequest.ToCountry();
        
        country.CountryId = Guid.NewGuid();
        
        await _countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return (await _countriesRepository.GetAllCountries())
            .Select(c => c.ToCountryResponse()).ToList();
    }
    
    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId is null)
            return null;
        
        Country? country = await _countriesRepository
            .GetCountryByCountryId(countryId.Value);

        if (country is null)
            return null;

        return country.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        MemoryStream memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        int countriesInserted = 0;
        
        ExcelPackage.License.SetNonCommercialPersonal("Leonidas");
        using ExcelPackage excelPackage = new ExcelPackage(memoryStream);
        ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];

        int rowCount = excelWorksheet.Dimension.Rows;
            

        for (var i = 2; i <= rowCount; i++)
        {
            var cellValue = excelWorksheet.Cells[i, 1].Value.ToString();
            if (string.IsNullOrEmpty(cellValue)) continue;

            if (await _countriesRepository.GetCountryByCountryName(cellValue)
                is not null) continue;
            
            Country countryAddRequest = new()
            {
                Name = cellValue, 
                CountryId = Guid.NewGuid()
            };
            await _countriesRepository.AddCountry(countryAddRequest);
            countriesInserted++;
        }

        return countriesInserted;
    }
}