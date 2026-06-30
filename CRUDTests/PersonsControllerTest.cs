using AutoFixture;
using Moq;
using ServiceContracts;
using FluentAssertions;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests;

public class PersonsControllerTest
{
    private readonly Mock<ICountryService> _mockCountryService;
    private readonly Mock<IPersonService> _mockPersonService;
    private readonly IFixture _fixture;
    private readonly PersonsController _personsController;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();

        _mockCountryService = new Mock<ICountryService>();
        _mockPersonService = new Mock<IPersonService>();

        var logger = new Mock<ILogger<PersonsController>>();
        var countryService = _mockCountryService.Object;
        var personService = _mockPersonService.Object;
        _personsController = new PersonsController(personService, countryService, logger.Object);
    }


    #region Index

    [Fact]
    public async Task Index_ReturnIndexViewWithPersonsList()
    {
        // Arrange
        var personResponses = _fixture.Create<List<PersonResponse>>();
        var personsController = _personsController;

        _mockPersonService.Setup(t => t.GetFilteredPersons(It.IsAny<string>(),
            It.IsAny<string?>())).ReturnsAsync(personResponses);
        _mockPersonService.Setup(t => t.GetSortedPersons(It.IsAny<List<PersonResponse>>(),
            It.IsAny<string>(), It.IsAny<SortOrderEnum>())).ReturnsAsync(personResponses);
        
        // Act
        IActionResult result = await personsController
            .Index(_fixture.Create<string>(), _fixture.Create<string>());
        
        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        
        viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personResponses);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_Parameterless()
    {
        List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();
        var personsController = _personsController;

        _mockCountryService.Setup(t => t.GetAllCountries()).ReturnsAsync(countryResponses);

        var result = personsController.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_WithValidationErrors_ToReturnCreateView()
    {
        var personAddRequest = _fixture.Create<PersonAddRequest>();
        var personResponse = _fixture.Create<PersonResponse>();
        var countries = _fixture.Create<List<CountryResponse>>();
        
        var personsController = _personsController;
        personsController.ModelState.AddModelError("PersonName", "You did not");
        
        _mockCountryService.Setup(t => t.GetAllCountries()).ReturnsAsync(countries);
        _mockPersonService.Setup(t => t.AddPersonAsync(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        var result = await personsController.Create(personAddRequest);

        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.ViewData["errors"].Should().NotBeNull();
        viewResult.ViewName.Should().Be("Create");
    }
    
    [Fact]
    public async Task Create_NoValidationErrors_ToReturnIndexView()
    {
        var personAddRequest = _fixture.Create<PersonAddRequest>();
        var personResponse = _fixture.Create<PersonResponse>();
        var countries = _fixture.Create<List<CountryResponse>>();
        
        var personsController = _personsController;
        
        _mockCountryService.Setup(t => t.GetAllCountries()).ReturnsAsync(countries);
        _mockPersonService.Setup(t => t.AddPersonAsync(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        var result = await personsController.Create(personAddRequest);

        result.Should().BeOfType<RedirectToActionResult>();
    }

    #endregion
}