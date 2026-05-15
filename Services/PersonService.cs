using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonService : IPersonService
{
    private readonly ICountryService _countryService;
    private readonly List<Person> _persons;

    public PersonService(bool initialize = true)
    {
        _countryService = new CountriesService();
        _persons = [];
        if (initialize)
        {
            // 3DADD97E-95E5-4305-BD43-71F745323BD5
            // F5CC911A-A61B-4314-9719-0B10E3DC7468
            // FE3CB2A2-CE32-4AE7-BFCE-744C61768787
            // 46B76EDB-2311-491A-B95A-24FFD5C702C9
            // F08DE97B-D251-43BE-BA53-DB163ADD2586
            // 90651C4D-B1FA-43CD-955B-C41ED9F8BAB6
            _persons.AddRange();
        }
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
        
        // Validate properties
        // if (string.IsNullOrEmpty(personAddRequest.Name))
        //     throw new ArgumentException();
        
        /* Better use model validation */
        ValidationHelper.ModelValidation(personAddRequest);

        // Convert
        Person person = personAddRequest.ToPerson(Guid.NewGuid());
        
        // Add the Person to the person list
        _persons.Add(person);
        
        // Return the PersonResponse
        PersonResponse responsePerson = ConvertPersonToPersonResponse(person);

        return responsePerson;
    }

    public PersonResponse? GetPersonById(Guid? personId)
    {
        if (personId == null)
            return null;
        
        PersonResponse? personResponse = _persons
            .FirstOrDefault(p => p.PersonId == personId)?.ToPersonResponse();
        if (personResponse is null)
            return null;

        // TODO Check if this works
        personResponse.Country = _countryService
            .GetCountryByCountryId(personResponse.CountryId)?.Name;
        return personResponse;
        
        // return personId is null ? null 
        //     : _persons.FirstOrDefault(p => p.PersonId == personId)
        //         ?.ToPersonResponse();
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(p => p.ToPersonResponse()).ToList();
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        var allPersons = GetAllPersons();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return allPersons;

        List<PersonResponse> matchingPersons = searchBy switch
        {
            nameof(Person.Name) => allPersons.Where(p => 
                string.IsNullOrEmpty(p.Name) || p.Name.Contains(searchString,
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(Person.Email) => allPersons.Where(p => 
                string.IsNullOrEmpty(p.Email) || p.Email.Contains(searchString, 
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(Person.DateOfBirth) => allPersons.Where(p => 
                p.DateOfBirth == null || p.DateOfBirth.Value.ToString("dd MMMM yyyy")
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(Person.Gender) => allPersons.Where(p => 
                p.Gender is null || p.Gender.ToString()!.Contains(searchString, 
                    StringComparison.OrdinalIgnoreCase)).ToList(),
            
            nameof(Person.Address) => allPersons.Where(p => 
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
                nameof(Person.Name) => allPersons.OrderBy(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(Person.Email) => allPersons.OrderBy(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(Person.DateOfBirth) => allPersons.OrderBy(p => p.DateOfBirth).ToList(),
                nameof(Person.Gender) => allPersons.OrderBy(p => p.Gender).ToList(),
                nameof(Person.Address) => allPersons.OrderBy(p => p.Address,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                _ => allPersons
            },
            SortOrderEnum.Descending => sortBy switch
            {
                nameof(Person.Name) => allPersons.OrderByDescending(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(Person.Email) => allPersons.OrderByDescending(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(Person.DateOfBirth) => allPersons.OrderByDescending(p =>
                    p.DateOfBirth).ToList(),
                nameof(Person.Gender) => allPersons.OrderByDescending(p =>
                    p.Gender).ToList(),
                nameof(Person.Address) => allPersons.OrderByDescending(p =>
                    p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
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

        Person? matchingPerson = _persons.FirstOrDefault(p => 
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

        return matchingPerson.ToPersonResponse();
    }

    public bool DeletePerson(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));

        Person? personToDelete = _persons.FirstOrDefault(p => p.PersonId == personId);
        if (personToDelete is null)
            return false;

        var removed = _persons.RemoveAll(p => p.PersonId == personId);
        return removed > 0;
    }
}