using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class PersonServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountryService _countryService;
    private readonly ITestOutputHelper _outputHelper;

    public PersonServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _personService = new PersonService(false);
        _countryService = new CountriesService(false);
    }

    // Helper Method to add a few countries and persons
    private List<PersonResponse> AddSomePersons()
    {
        CountryAddRequest countryAddRequest1 = new() { CountryName = "Greece" };
        CountryAddRequest countryAddRequest2 = new() { CountryName = "England" };
        CountryResponse countryResponse1 = _countryService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = _countryService.AddCountry(countryAddRequest2);

        List<PersonAddRequest> personAddRequests = [
            new() {
                Name = "Leonidas",
                Email = "leonidas@gmail.com",
                DateOfBirth = new DateTime(1989, 10, 5),
                Gender = GenderOptions.Male,
                CountryId = countryResponse1.CountryId,
                Address = "Papandreou",
                ReceiveNewsLetters = true
            },
            new() {
                Name = "Takis",
                Email = "ttakis@gmail.com",
                DateOfBirth = new DateTime(1889, 11, 5),
                Gender = GenderOptions.Female,
                CountryId = countryResponse2.CountryId,
                Address = "Edw",
                ReceiveNewsLetters = false
            },
            new() {
                Name = "Swtiris",
                Email = "swtos@gmail.com",
                DateOfBirth = new DateTime(2001, 1, 4),
                Gender = GenderOptions.Other,
                CountryId = countryResponse2.CountryId,
                Address = "Kapou",
                ReceiveNewsLetters = false
            }
        ];
        
        
        _outputHelper.WriteLine("Added PersonResponse objects => ");
        List<PersonResponse> expectedPersons = [];
        foreach (PersonAddRequest addRequest in personAddRequests)
        {
            var temp = _personService.AddPerson(addRequest);
            expectedPersons.Add(temp);
            _outputHelper.WriteLine(temp.ToString());
        }

        return expectedPersons;
    }
    
    #region AddPerson()

    // Supply a null value as PersonAddRequest, return ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _personService.AddPerson(personAddRequest);
        });
    }
    
    // Supply a null value as Name, return ArgumentException
    [Fact]
    public void AddPerson_NullPersonName()
    {
        // Arrange
        PersonAddRequest personAddRequest = new() { Name = null };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personService.AddPerson(personAddRequest);
        });
    }
    
    // Supply a proper PersonAddRequest, it should insert it in the list and return
    // the appropriate PersonResponse
    [Fact]
    public void AddPerson_ProperPerson()
    {
        // Arrange
        PersonAddRequest personAddRequest = new() {
            Name = "Leonidas",
            Email = "leonidas@gmail.com",
            DateOfBirth = new DateTime(1989, 10, 5),
            Gender = GenderOptions.Male,
            CountryId = Guid.NewGuid(),
            Address = "Papandreou",
            ReceiveNewsLetters = true
        };
        
        // Act
        PersonResponse personResponse = _personService.AddPerson(personAddRequest);
        List<PersonResponse> personList = _personService.GetAllPersons();

        // Assert
        Assert.True(personResponse.PersonId != Guid.Empty);
        Assert.Contains(personResponse, personList);
    }

    #endregion

    #region GetAllPersons()

    [Fact]
    public void GetAllPersons_EmptyList()
    {
        // Arrange - Act
        List<PersonResponse> personResponses = _personService.GetAllPersons();

        // Assert
        Assert.Empty(personResponses);
    }

    [Fact]
    public void GetAllPersons_NonEmptyList()
    {
        // Arrange
        CountryAddRequest countryAddRequest1 = new() { CountryName = "Greece" };
        CountryAddRequest countryAddRequest2 = new() { CountryName = "England" };
        CountryResponse countryResponse1 = _countryService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = _countryService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = new() {
            Name = "Leonidas",
            Email = "leonidas@gmail.com",
            DateOfBirth = new DateTime(1989, 10, 5),
            Gender = GenderOptions.Male,
            CountryId = countryResponse1.CountryId,
            Address = "Papandreou",
            ReceiveNewsLetters = true
        };
        PersonAddRequest personAddRequest2 = new() {
            Name = "Takis",
            Email = "ttakis@gmail.com",
            DateOfBirth = new DateTime(1889, 11, 5),
            Gender = GenderOptions.Female,
            CountryId = countryResponse2.CountryId,
            Address = "Edw",
            ReceiveNewsLetters = false
        };
        
        var p1 = _personService.AddPerson(personAddRequest1);
        var p2 = _personService.AddPerson(personAddRequest2);

        // Act
        List<PersonResponse> actualPersons = _personService.GetAllPersons();

        // Assert
        Assert.Contains(p1, actualPersons);
        Assert.Contains(p2, actualPersons);
    }
    #endregion

    #region GetPersonById()

    [Fact]
    public void GetPersonById_NullPersonId()
    {
        Guid? personId = null;

        var personResponse = _personService.GetPersonById(personId);
        
        Assert.Null(personResponse);
    }

    [Fact]
    public void GetPersonById_ProperPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Greece" };
        CountryResponse countryResponse = _countryService.AddCountry(countryAddRequest);
        
        PersonAddRequest personAddRequest = new() {
            Name = "Leonidas",
            Email = "leonidas@gmail.com",
            DateOfBirth = new DateTime(1979, 10, 5),
            Gender = GenderOptions.Male,
            CountryId = countryResponse.CountryId,      // Use the Guid of the created country
            Address = "Papandreou",
            ReceiveNewsLetters = true
        };
        PersonResponse personResponse = _personService.AddPerson(personAddRequest);     // Expected
        PersonResponse? actualPerson = _personService.GetPersonById(personResponse.PersonId);   // Actual

        Assert.Equal(actualPerson, personResponse);
    }
    
    [Fact]
    public void GetPersonById_ProperPersonIdNotExisting()
    {
        PersonResponse? actualPerson = _personService.GetPersonById(Guid.NewGuid());   // Actual
        
        Assert.Null(actualPerson);
    }

    #endregion

    #region GetFilteredPersons()

    // if search text is empty and search by "Name", it should return all persons
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
    {
        // Arrange
        List<PersonResponse> expectedPersons = AddSomePersons();
        
        // Act
        List<PersonResponse> actualPersons = 
            _personService.GetFilteredPersons(nameof(Person.Name), "");
        foreach (PersonResponse person in actualPersons)
        {
            _outputHelper.WriteLine(person.ToString());
        }
        
        // Assert
        foreach (var person in expectedPersons)
        {
            Assert.Contains(person, actualPersons);
        }
    }

    // Add few countries, persons and search based on person Name with search string
    // must return appropriate persons !case-insensitive
    [Fact]
    public void GetFilteredPersons_ProperText()
    {
        // Arrange
        List<PersonResponse> expectedPersons = AddSomePersons();

        // Act
        List<PersonResponse> actualPersons = 
            _personService.GetFilteredPersons(nameof(Person.Name), "l");
        _outputHelper.WriteLine("\n\nActual: ");
        foreach (PersonResponse person in actualPersons)
        {
            _outputHelper.WriteLine(person.ToString());
        }
        
        // Assert
        foreach (var person in expectedPersons)
        {
            if (person.Name is null) continue;
            if (person.Name.Contains("l", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Contains(person, actualPersons);
            }
        }
    }
    
    #endregion

    #region GetSortedPersons()

    // Sort on person Name with descending order
    [Fact]
    public void GetSortedPersons_Descending()
    {
        // Arrange
        List<PersonResponse> expectedPersons = AddSomePersons();
        List<PersonResponse> allPersons = _personService.GetAllPersons();
        
        // Act
        List<PersonResponse> actualPersons = 
            _personService.GetSortedPersons(allPersons, nameof(Person.Name), 
                SortOrderEnum.Descending);
        _outputHelper.WriteLine("\n\nActual: ");
        foreach (PersonResponse person in actualPersons)
        {
            _outputHelper.WriteLine(person.ToString());
        }

        expectedPersons = expectedPersons.OrderByDescending(p => p.Name).ToList();
        // Assert
        for (int i = 0; i < actualPersons.Count; i++)
        {
            Assert.Equal(expectedPersons[i], actualPersons[i]);
        }
    }
    
    // Sort on person Name with ascending order
    [Fact]
    public void GetSortedPersons_Ascending()
    {
        // Arrange
        List<PersonResponse> expectedPersons = AddSomePersons();
        List<PersonResponse> allPersons = _personService.GetAllPersons();
        
        // Act
        List<PersonResponse> actualPersons = 
            _personService.GetSortedPersons(allPersons, nameof(Person.Name), 
                SortOrderEnum.Ascending);
        _outputHelper.WriteLine("\n\nActual: ");
        foreach (PersonResponse person in actualPersons)
        {
            _outputHelper.WriteLine(person.ToString());
        }

        expectedPersons = expectedPersons.OrderBy(p => p.Name).ToList();
        // Assert
        for (int i = 0; i < actualPersons.Count; i++)
        {
            Assert.Equal(expectedPersons[i], actualPersons[i]);
        }
    }

    #endregion

    #region UpdatePerson()

    // When supplied null PersonUpdateRequest throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPersonUpdateRequest()
    {
        // Arrange
        PersonUpdateRequest? personUpdate = null;
        
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _personService.UpdatePerson(personUpdate);
        });
    }
    
    // When supplied invalid Person Id throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonId()
    {
        // Arrange
        PersonUpdateRequest personUpdate = new PersonUpdateRequest() 
            { PersonId = Guid.NewGuid() };
        
        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personService.UpdatePerson(personUpdate);
        });
    }
    
    // When the Person Name is null
    [Fact]
    public void UpdatePerson_NullPersonName()
    {
        // Arrange
        var personsAdded = AddSomePersons();
        PersonUpdateRequest personUpdateRequest = personsAdded[0].ToPersonUpdateRequest();
        personUpdateRequest.Name = null;
        
        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personService.UpdatePerson(personUpdateRequest);
        });
    }
    
    // When proper PersonUpdateRequest
    [Fact]
    public void UpdatePerson_ProperRequest()
    {
        // Arrange
        var personsAdded = AddSomePersons();
        _outputHelper.WriteLine("Original => ");
        _outputHelper.WriteLine(personsAdded[0].ToString());
        PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = personsAdded[0].PersonId,
            Name = "Mamalakis",
            Email = "mamalakis@example.com",
            DateOfBirth = new DateTime(2005, 5, 5),
            Gender = GenderOptions.Other,
            Address = "Ioannina",
            ReceiveNewsLetters = true
        };
        
        // Act
        var actual = _personService.UpdatePerson(personUpdateRequest);
        _outputHelper.WriteLine("Actual => ");
        _outputHelper.WriteLine(actual.ToString());
        var expected = _personService.GetPersonById(actual.PersonId);
        _outputHelper.WriteLine("Expected => ");
        _outputHelper.WriteLine(expected?.ToString() ?? "NULL");
        
        // Assert
        Assert.Equal(actual, expected);
    }

    #endregion

    #region DeletePerson()

    // If you supply null PersonId
    [Fact]
    public void DeletePerson_NullPersonId()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personService.DeletePerson(null);
        });
    }
    
    // if you supply an invalid PersonId return false
    [Fact]
    public void DeletePerson_InvalidPersonId()
    {
        Assert.False(_personService.DeletePerson(Guid.NewGuid()));
    }
    
    // If you supply a valid PersonId return true
    [Fact]
    public void DeletePerson_ValidPersonId()
    {
        var addedPerson = AddSomePersons();
        var personToDelete = addedPerson[0];
        bool isDeleted = _personService.DeletePerson(personToDelete.PersonId);
        
        Assert.True(isDeleted);

        var finalList = _personService.GetAllPersons();
        Assert.DoesNotContain(personToDelete, finalList);
    }

    #endregion
}