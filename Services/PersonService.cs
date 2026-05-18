using Entities;
using Microsoft.EntityFrameworkCore.Internal;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonService : IPersonService
{
    private readonly PersonsDbContext _dbContext;
    private readonly ICountryService _countryService;

    public PersonService(PersonsDbContext dbContext, ICountryService countryService)
    {
        _dbContext = dbContext;
        _countryService = countryService;
    }

    /// <summary>
    /// Helper method to convert a Person to PersonResponse type and
    /// add the name of country calling the ICountriesService
    /// </summary>
    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse responsePerson = person.ToPersonResponse();
        responsePerson.Country = _countryService.GetCountryByCountryId(person.CountryId)?.Name;
        return responsePerson;
    }
    
    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        // Check if personAddRequest is null
        if (personAddRequest is null)
            throw new ArgumentNullException();
        
        /* Better use model validation */
        ValidationHelper.ModelValidation(personAddRequest);

        // Convert
        Person person = personAddRequest.ToPerson(Guid.NewGuid());
        
        // Add the Person to the person list
        // _dbContext.Persons.Add(person);
        // _dbContext.SaveChanges();
        _dbContext.sp_InsertPerson(person);
        
        // Return the PersonResponse
        PersonResponse responsePerson = ConvertPersonToPersonResponse(person);

        return responsePerson;
    }

    public PersonResponse? GetPersonById(Guid? personId)
    {
        if (personId == null)
            return null;
        
        Person? person = _dbContext.Persons
            .FirstOrDefault(p => p.PersonId == personId);
        
        if (person is null)
            return null;

        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        // Using Stored Procedure
        return _dbContext.sp_GetAllPersons()
            .Select(ConvertPersonToPersonResponse).ToList();
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        var allPersons = GetAllPersons();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return allPersons;

        List<PersonResponse> matchingPersons = searchBy switch
        {
            nameof(PersonResponse.Name) => allPersons.Where(p => 
                string.IsNullOrEmpty(p.Name) || p.Name.Contains(searchString,
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(PersonResponse.Email) => allPersons.Where(p => 
                string.IsNullOrEmpty(p.Email) || p.Email.Contains(searchString, 
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(PersonResponse.DateOfBirth) => allPersons.Where(p => 
                p.DateOfBirth == null || p.DateOfBirth.Value.ToString("dd MMMM yyyy")
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(PersonResponse.Gender) => allPersons.Where(p => 
                p.Gender is null || p.Gender.ToString()!.Contains(searchString, 
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(PersonResponse.Address) => allPersons.Where(p => 
                string.IsNullOrEmpty(p.Address) || p.Address.Contains(searchString, 
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            _ => allPersons
        };
        
        return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allPersons;

        List<PersonResponse> sortedPersons = sortOrder switch
        {
            SortOrderEnum.Ascending => sortBy switch
            {
                nameof(PersonResponse.Name) => allPersons.OrderBy(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Email) => allPersons.OrderBy(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.DateOfBirth) => allPersons.OrderBy(p => p.DateOfBirth).ToList(),
                nameof(PersonResponse.Gender) => allPersons.OrderBy(p => p.Gender).ToList(),
                nameof(PersonResponse.Address) => allPersons.OrderBy(p => p.Address,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Age) => allPersons.OrderBy(p => p.Age).ToList(),
                nameof(PersonResponse.Country) => allPersons.OrderBy(p => p.Country, StringComparer
                .OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.ReceiveNewsLetters) => allPersons.OrderBy(p => p.ReceiveNewsLetters).ToList(),
                _ => allPersons
            },
            SortOrderEnum.Descending => sortBy switch
            {
                nameof(PersonResponse.Name) => allPersons.OrderByDescending(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Email) => allPersons.OrderByDescending(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.DateOfBirth) => allPersons.OrderByDescending(p =>
                    p.DateOfBirth).ToList(),
                nameof(PersonResponse.Gender) => allPersons.OrderByDescending(p =>
                    p.Gender).ToList(),
                nameof(PersonResponse.Address) => allPersons.OrderByDescending(p =>
                    p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Age) => allPersons.OrderByDescending(p => p.Age).ToList(),
                nameof(PersonResponse.Country) => allPersons.OrderByDescending(p => p.Country, StringComparer
                    .OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.ReceiveNewsLetters) => allPersons.OrderByDescending(p => p.ReceiveNewsLetters).ToList(),
                _ => allPersons
            },
            _ => allPersons
        };
        
        return sortedPersons;
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest is null)
            throw new ArgumentNullException();
        
        ValidationHelper.ModelValidation(personUpdateRequest);

        Person? matchingPerson = _dbContext.Persons.FirstOrDefault(p => 
            p.PersonId == personUpdateRequest.PersonId);

        if (matchingPerson is null)
            throw new ArgumentException("Given person Id does not exist");

        matchingPerson.Name = personUpdateRequest.Name ?? matchingPerson.Name;
        matchingPerson.Email = personUpdateRequest.Email ?? matchingPerson.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth ?? matchingPerson.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString() ?? matchingPerson.Gender;
        matchingPerson.CountryId = personUpdateRequest.CountryId ?? matchingPerson.CountryId;
        matchingPerson.Address = personUpdateRequest.Address ?? matchingPerson.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        _dbContext.SaveChanges(); // UPDATE
        
        return ConvertPersonToPersonResponse(matchingPerson);
    }

    public bool DeletePerson(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));

        Person? personToDelete = _dbContext.Persons.FirstOrDefault(p => p.PersonId == personId);
        if (personToDelete is null)
            return false;

        _dbContext.Persons.Remove(_dbContext.Persons.First(p => p.PersonId == personId));
        var num = _dbContext.SaveChanges();
        return num > 1;
    }
}