using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public IActionResult Create()
    {
        var countries = _countryService.GetAllCountries();
        ViewBag.Countries = countries.Select(c => new SelectListItem() 
        {
            Text = c.Name,
            Value = c.CountryId.ToString()
        });
        
        return View();
    }

    [HttpPost]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        var countries = _countryService.GetAllCountries();
        ViewBag.Countries = countries.Select(c => new SelectListItem() 
        {
            Text = c.Name,
            Value = c.CountryId.ToString()
        });
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

    [HttpGet]
    [Route("{personId}")]
    public IActionResult Edit(Guid personId)
    {
        PersonResponse? person = _personService.GetPersonById(personId);
        if (person is null)
            return RedirectToAction("Index");

        var countries = _countryService.GetAllCountries();
        ViewBag.Countries = countries.Select(c => new SelectListItem() 
        {
            Text = c.Name,
            Value = c.CountryId.ToString()
        });
        
        PersonUpdateRequest updateRequest = person.ToPersonUpdateRequest();
        
        return View(updateRequest);
    }

    [HttpPost]
    [Route("{personId}")]
    public IActionResult Edit(PersonUpdateRequest personUpdate)
    {
        _personService.UpdatePerson(personUpdate);
        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("{personId}")]
    public IActionResult Delete(Guid personId)
    {
        PersonResponse? personToDelete = _personService.GetPersonById(personId);
        if (personToDelete is null)
            return RedirectToAction("Index", "Persons");
        
        return View(personToDelete);
    }

    [HttpPost]
    [Route("{personId}")]
    public IActionResult Delete(PersonResponse person)
    {
        _personService.DeletePerson(person.PersonId);
        return RedirectToAction("Index", "Persons");
    }
}