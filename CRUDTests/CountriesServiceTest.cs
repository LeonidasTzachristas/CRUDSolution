using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountryService _countryService;
    private readonly ITestOutputHelper _outputHelper;

    public CountriesServiceTest(ITestOutputHelper outputHelper)
    {
        _countryService = new CountriesService(false);
        _outputHelper = outputHelper;
    }

    #region AddCountry()
    /* Four possible scenarios */
    // 1. When CountryAddRequest is null, it should throw ArgumentNullException
    /// <summary>
    /// Test for AddCountry if the country <b>is null</b>
    /// </summary>
    [Fact]
    public void AddCountry_NullCountry()
    {
        // Arrange
        CountryAddRequest? request = null;
        
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _countryService.AddCountry(request);
        });
    }
    
    // 2. When the CountryName is null, it should throw ArgumentException
    [Fact]
    public void AddCountry_NullName()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest() { CountryName = null };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _countryService.AddCountry(request);
        });
    }
    // 3. When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        // Arrange
        CountryAddRequest request1 = new CountryAddRequest() { CountryName = "Greece" };
        CountryAddRequest request2 = new CountryAddRequest() { CountryName = "Greece" };
        
        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _countryService.AddCountry(request1);
            _countryService.AddCountry(request2);
        });
    }
    // 4. When the CountryAddRequest is proper it should add the country to the existing list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        // Arrange
        CountryAddRequest request = new CountryAddRequest() { CountryName = "Greece" };
        
        // Act
        var response = _countryService.AddCountry(request);
        var allCountries = _countryService.GetAllCountries();
        
        // Assert
        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, allCountries);    // Ensure the created country is added in the list
    }
    #endregion

    #region GetAllCountries()

    // The List should be empty by default at the start
    [Fact]
    public void GetAllCountries_EmptyList()
    {
        // Act
        List<CountryResponse> actualCountryResponseList = 
            _countryService.GetAllCountries();
        
        // Assert
        Assert.Empty(actualCountryResponseList);
    }
    
    // Return the added countries
    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
        // Arrange
        List<CountryAddRequest> countryAddRequests = [
            new CountryAddRequest() {CountryName = "Greece"},
            new CountryAddRequest() {CountryName = "Albania"},
            new CountryAddRequest() {CountryName = "Italy"}
        ];
        
        List<CountryResponse> countryResponses = [];
        
        foreach (CountryAddRequest request in countryAddRequests)
        {
            countryResponses.Add(_countryService.AddCountry(request));
        }

        List<CountryResponse> actualCountryResponses =
            _countryService.GetAllCountries();

        foreach (var expectedCountry in countryResponses)
        {
            Assert.Contains(expectedCountry, actualCountryResponses);
        }
    }
    #endregion

    #region GetCountryByCountryId()

    // Supply a null Id
    [Fact]
    public void GetCountryByCountryId_NullCountryId()
    {
        // Arrange
        Guid? countryId = null;
        
        // Act - Assert
        Assert.Null(_countryService.GetCountryByCountryId(countryId));
    }

    // Supply a valid Guid value and get the details of matching country
    [Fact]
    public void GetCountryByCountryId_WithMatchingValue()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new() { CountryName = "Greece"};
        CountryResponse countryResponse = _countryService.AddCountry(countryAddRequest);
        
        // Act
        CountryResponse? actualCountry = _countryService.GetCountryByCountryId(countryResponse.CountryId);
        _outputHelper.WriteLine(actualCountry?.ToString());
        // Assert
        Assert.Equal(countryResponse, actualCountry);
    }
    
    [Fact]
    public void GetCountryByCountryId_WithNonMatchingValue()
    {
        // Arrange
        Guid unmatched = Guid.NewGuid();
        CountryAddRequest countryAddRequest = new() { CountryName = "Greece"};
        CountryResponse countryResponse = _countryService.AddCountry(countryAddRequest);
        
        // Act
        CountryResponse? actualCountry = _countryService.GetCountryByCountryId(unmatched);
        
        // Assert
        Assert.Null(actualCountry);
    }
    
    #endregion

}