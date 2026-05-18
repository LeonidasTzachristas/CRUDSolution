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
            _persons.AddRange(
                new Person() {
                    PersonId = Guid.Parse("3DADD97E-95E5-4305-BD43-71F745323BD5"), Name = "Ketty", 
                    Email = "kbrane0@vimeo.com", DateOfBirth = DateTime.Parse("1984-02-21"), Gender = "Female", 
                    CountryId = Guid.Parse("A3F84C4A-8CE2-4A15-B560-D4DFEB09B328"),
                    Address = "972 Merrick Terrace", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("F5CC911A-A61B-4314-9719-0B10E3DC7468"), Name = "Bradney", 
                    Email = "bbabidge1@guardian.co.uk", DateOfBirth = DateTime.Parse("1999-06-04"), Gender = "Male", 
                    CountryId = Guid.Parse("A3F84C4A-8CE2-4A15-B560-D4DFEB09B328"),
                    Address = "048 Heffernan Crossing", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("FE3CB2A2-CE32-4AE7-BFCE-744C61768787"), Name = "Rupert", 
                    Email = "ryakovich2@vistaprint.com", DateOfBirth = DateTime.Parse("1996-06-27"), Gender = "Male", 
                    CountryId = Guid.Parse("0E241979-E826-47E9-837F-C0533E5F88FF"),
                    Address = "9 Arkansas Circle", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("46B76EDB-2311-491A-B95A-24FFD5C702C9"), Name = "Shawnee", 
                    Email = "spauleau3@liveinternet.ru", DateOfBirth = DateTime.Parse("2000-08-07"), Gender = "Female", 
                    CountryId = Guid.Parse("4FFA4035-1DDF-4005-A72A-48FF4D47F7AB"),
                    Address = "7722 Carpenter Road", ReceiveNewsLetters = true
                },
                new Person() {
                    PersonId = Guid.Parse("F08DE97B-D251-43BE-BA53-DB163ADD2586"), Name = "Siegfried", 
                    Email = "sdignam4@thetimes.co.uk", DateOfBirth = DateTime.Parse("1985-03-05"), Gender = "Male", 
                    CountryId = Guid.Parse("657D8B4A-FBA4-4B99-AD17-F738DD7D8F7F"),
                    Address = "4488 Cottonwood Alley", ReceiveNewsLetters = true
                },
                new Person() {
                    PersonId = Guid.Parse("90651C4D-B1FA-43CD-955B-C41ED9F8BAB6"), Name = "Torin", 
                    Email = "tprall5@yellowbook.com", DateOfBirth = DateTime.Parse("1997-09-11"), Gender = "Male", 
                    CountryId = Guid.Parse("D04E3EA8-E52E-480C-9ABF-15D2D97586B9"),
                    Address = "55 Amoth Point", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("62789AB8-DE08-45DD-A5E7-CFCF27803EF8"), Name = "Nat", 
                    Email = "nfawthrop6@imdb.com", DateOfBirth = DateTime.Parse("1990-11-06"), Gender = "Female", 
                    CountryId = Guid.Parse("4FFA4035-1DDF-4005-A72A-48FF4D47F7AB"),
                    Address = "841 Scoville Place", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("A379DF93-A4FD-4C6A-AD6A-FB3D010F48C2"), Name = "Justus", 
                    Email = "jorrill7@statcounter.com", DateOfBirth = DateTime.Parse("1989-09-27"), Gender = "Male", 
                    CountryId = Guid.Parse("9254EEF2-45B1-4F09-9A7D-423A9E582A8C"),
                    Address = "52 Crescent Oaks Junction", ReceiveNewsLetters = true
                },
                new Person() {
                    PersonId = Guid.Parse("1E9E0C61-D084-4CE6-A368-7C7AC6FEFB45"), Name = "Ky", 
                    Email = "kdomnin8@reverbnation.com", DateOfBirth = DateTime.Parse("1993-04-05"), Gender = "Male", 
                    CountryId = Guid.Parse("0E241979-E826-47E9-837F-C0533E5F88FF"),
                    Address = "83511 Fallview Road", ReceiveNewsLetters = false
                },
                new Person() {
                    PersonId = Guid.Parse("3E32A158-BD3E-4F5C-B229-B912FDFD6A1B"), Name = "Teena", 
                    Email = "tcomford9@cocolog-nifty.com", DateOfBirth = DateTime.Parse("1981-06-01"), Gender = "Female", 
                    CountryId = Guid.Parse("D04E3EA8-E52E-480C-9ABF-15D2D97586B9"),
                    Address = "5019 Moose Drive", ReceiveNewsLetters = true
                });
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
        
        Person? person = _persons.FirstOrDefault(p => p.PersonId == personId);
        
        if (person is null)
            return null;

        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(ConvertPersonToPersonResponse).ToList();
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

        return ConvertPersonToPersonResponse(matchingPerson);
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