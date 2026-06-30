using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesRepository _countriesRepository;
    private readonly Mock<ICountriesRepository> _mockCountriesRepository;
    
    private readonly ICountryService _countryService;
    private readonly ITestOutputHelper _outputHelper;

    public CountriesServiceTest(ITestOutputHelper outputHelper)
    {
        _mockCountriesRepository = new Mock<ICountriesRepository>();
        _countriesRepository = _mockCountriesRepository.Object;
        
        _countryService = new CountriesService(_countriesRepository);
        
        _outputHelper = outputHelper;
    }

    #region AddCountry()
    /* Four possible scenarios */
    // 1. When CountryAddRequest is null, it should throw ArgumentNullException
    /// <summary>
    /// Test for AddCountry if the country <b>is null</b>
    /// </summary>
    [Fact]
    public async Task AddCountry_NullCountry_ThrowArgumentNullException()
    {
        // Arrange
        CountryAddRequest? request = null;
        
        // Act
        Func<Task> action = async () =>
            await _countryService.AddCountry(request);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    // 2. When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_NullName_ThrowArgumentException()
    {
        // Arrange
        CountryAddRequest request = new() { CountryName = null };

        // Act
        Func<Task> action = async () =>
            await _countryService.AddCountry(request);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    // 3. When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        // Arrange
        CountryAddRequest request = new() { CountryName = "Greece" };
        var country = request.ToCountry();

        _mockCountriesRepository.Setup(t => t.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(country);
        
        // Act
        Func<Task> action = async () =>
        {
            await _countryService.AddCountry(request);
        };
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    // 4. When the CountryAddRequest is proper it should add the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        // Arrange
        CountryAddRequest request = new() { CountryName = "Greece" };
        Country country = request.ToCountry();
        
        _mockCountriesRepository.Setup(t => t.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(null as Country);
        
        // Act
        CountryResponse actualCountryResponse = await _countryService.AddCountry(request);
        
        // Assert
        actualCountryResponse.CountryId.Should().NotBe(Guid.Empty);

    }
    #endregion

    #region GetAllCountries()
    // The List should be empty by default at the start
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        _mockCountriesRepository.Setup(t => t.GetAllCountries()).ReturnsAsync([]);
        // Act
        List<CountryResponse> actualCountryResponseList = 
            await _countryService.GetAllCountries();
        
        // Assert
        actualCountryResponseList.Should().BeEmpty();
    }
    // Return the added countries
    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        // Arrange
        List<CountryAddRequest> countryAddRequests = [
            new() {CountryName = "Greece"},
            new() {CountryName = "Albania"},
            new() {CountryName = "Italy"}
        ];
        
        List<Country> countries = countryAddRequests.Select(c => c.ToCountry()).ToList();
        foreach (Country country in countries)
            country.CountryId = Guid.NewGuid();
        List<CountryResponse> expectedCountries = countries.Select(c => c.ToCountryResponse()).ToList();

        _mockCountriesRepository.Setup(t => t.GetAllCountries()).ReturnsAsync(countries);
        
        List<CountryResponse> actualCountryResponses =
            await _countryService.GetAllCountries();

        actualCountryResponses.Should().BeEqualTo(expectedCountries);
    }
    #endregion

    #region GetCountryByCountryId()
    // Supply a null Id
    [Fact]
    public async Task GetCountryByCountryId_NullCountryId_ToBeNull()
    {
        // Arrange
        Guid? countryId = null;
        
        // Act - Assert
        var country = await _countryService.GetCountryByCountryId(countryId);

        country.Should().BeNull();
    }

    // Supply a valid Guid value and get the details of matching country
    [Fact]
    public async Task GetCountryByCountryId_WithMatchingValue()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new() { CountryName = "Greece"};
        Country country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        _mockCountriesRepository.Setup(t => t.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(country);
        
        // Act
        CountryResponse? actualCountry = await _countryService
            .GetCountryByCountryId(country.CountryId);
        
        // Assert
        actualCountry.Should().Be(country.ToCountryResponse());
    }
    
    [Fact]
    public async Task GetCountryByCountryId_WithNonMatchingValue()
    {
        // Arrange
        Guid unmatched = Guid.NewGuid();
        CountryAddRequest countryAddRequest = new() { CountryName = "Greece"};
        CountryResponse countryResponse = await _countryService.AddCountry(countryAddRequest);
        
        // Act
        CountryResponse? actualCountry = await _countryService.GetCountryByCountryId(unmatched);
        
        // Assert
        // Assert.Null(actualCountry);
        actualCountry.Should().BeNull();
    }
    
    #endregion
}