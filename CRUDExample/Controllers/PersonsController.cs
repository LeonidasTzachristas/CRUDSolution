using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers;

[Route("[controller]/[action]")]
public class PersonsController : Controller
{
    private readonly IPersonService _personService;
    private readonly ICountryService _countryService;

    public PersonsController(IPersonService personService,
        ICountryService countryService)
    {
        _personService = personService;
        _countryService = countryService;
    }
    
    [Route("/")]
    [Route("")]
    public IActionResult Index(string searchBy, string? searchString,
        string sortBy = nameof(PersonResponse.Name), 
        SortOrderEnum sortOrder = SortOrderEnum.Ascending)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.Name), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.Address), "Address" },
        };
        var persons = _personService.GetFilteredPersons(searchBy, searchString);

        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = _personService
            .GetSortedPersons(persons, sortBy, sortOrder);

        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;
        
        return View(sortedPersons);
    }

    [HttpGet]
    // [Route("")]
    public IActionResult Create()
    {
        var countries = _countryService.GetAllCountries();
        ViewBag.Countries = countries;
        return View();
    }

    [HttpPost]
    // [Route("")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        var countries = _countryService.GetAllCountries();
        ViewBag.Countries = countries;
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            ViewBag.Errors = errors;
            return View();
        }

        var createdPerson = _personService.AddPerson(personAddRequest);
        return RedirectToAction("Index", "Persons");
    }
}