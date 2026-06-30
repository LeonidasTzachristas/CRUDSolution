using System.Linq.Expressions;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;

namespace CRUDTests;

public class PersonServiceTest
{
    private readonly Mock<IPersonsRepository> _mockPersonsRepository;
    
    private readonly IPersonService _personService;

    private readonly ITestOutputHelper _outputHelper;
    private readonly IFixture _fixture;

    public PersonServiceTest(ITestOutputHelper outputHelper)
    {
        _fixture = new Fixture();
        _mockPersonsRepository = new Mock<IPersonsRepository>();
        var personsRepository = _mockPersonsRepository.Object;
        var diagnosticContext = new Mock<IDiagnosticContext>();
        var logger = new Mock<ILogger<PersonService>>();
        
        _personService = new PersonService(personsRepository, logger.Object , diagnosticContext.Object);

        _outputHelper = outputHelper;
    }

    // Helper Method to add a few countries and persons
    private async Task<List<PersonResponse>> AddSomePersons()
    {
        List<Person> persons = [
            _fixture.Build<Person>()
                .With(p => p.Email, "example_1@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_2@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_3@example.com")
                .With(p => p.Country, null as Country).Create()
        ];
        var expectedPersons = persons.Select(p => p.ToPersonResponse()).ToList();

        return await expectedPersons.ToAsyncEnumerable().ToListAsync();
    }
    
    #region AddPerson()

    // Supply a null value as PersonAddRequest, return ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;
        
        // Act
        Func<Task> action = async () 
            => await _personService.AddPersonAsync(personAddRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    // Supply a null value as Name, return ArgumentException
    [Fact]
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
        // Arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Name, null as string).Create();

        Person person = personAddRequest.ToPerson();
        
        
        _mockPersonsRepository.Setup(t => t.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);
        
        // Act
        Func<Task> action = async () =>
            await _personService.AddPersonAsync(personAddRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    // Supply a proper PersonAddRequest, it should insert it in the list and return
    // the appropriate PersonResponse
    [Fact]
    public async Task AddPerson_ProperPerson_ToBeSuccessful()
    {
        // Arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com").Create();
        Person person = personAddRequest.ToPerson();
        PersonResponse personResponseExpected = person.ToPersonResponse();
        
        // Mock only the service method needed
        _mockPersonsRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(personAddRequest.ToPerson);
        
        // Act
        PersonResponse personResponseActual = await _personService.AddPersonAsync(personAddRequest);
        personResponseExpected.PersonId = personResponseActual.PersonId;
        
        // Assert
        personResponseActual.PersonId.Should().NotBe(Guid.Empty);
        personResponseActual.Should().Be(personResponseExpected);
    }

    #endregion

    #region GetAllPersons()

    [Fact]
    public async Task GetAllPersons_EmptyList_ToBeEmptyList()
    {
        _mockPersonsRepository.Setup(t => t.GetAllPersons()).ReturnsAsync([]);
        
        // Arrange - Act
        List<PersonResponse> personResponses = await _personService.GetAllPersons();

        // Assert
        personResponses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_NonEmptyList_ToBeSuccessful()
    {
        // Arrange
        List<Person> persons =
        [
            _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country).Create()
        ];
        List<PersonResponse> expectedPersons = persons.Select(p => p.ToPersonResponse()).ToList();

        _mockPersonsRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
        
        // Act
        List<PersonResponse> actualPersons = await _personService.GetAllPersons();

        // Assert
        actualPersons.Should().BeEqualTo(expectedPersons);
    }
    #endregion

    #region GetPersonById()

    [Fact]
    public async Task GetPersonById_NullPersonId_ToBeNull()
    {
        Guid? personId = null;

        var personResponse = await _personService.GetPersonById(personId);
        
        personResponse.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonById_ProperPersonId_ToBeSuccessful()
    {
        Person person = _fixture.Build<Person>()
            .With(p => p.Email, "example@example.com")
            .With(p => p.Country, null as Country).Create();
        PersonResponse personResponseExpected = person.ToPersonResponse();

        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        
        PersonResponse? actualPerson = await _personService.GetPersonById(person.PersonId);   // Actual

        actualPerson.Should().Be(personResponseExpected);
    }
    
    [Fact]
    public async Task GetPersonById_ProperPersonIdNotExisting_ToBeNull()
    {
        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync((Person?)null);
        
        PersonResponse? actualPerson = await _personService.GetPersonById(Guid.NewGuid());   // Actual
        
        actualPerson.Should().BeNull();
    }

    #endregion

    #region GetFilteredPersons()

    // if search text is empty and search by "Name", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
        // Arrange
        List<Person> persons = [
            _fixture.Build<Person>()
                .With(p => p.Email, "example_1@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_2@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_3@example.com")
                .With(p => p.Country, null as Country).Create()
        ];
        List<PersonResponse> expectedPersons = persons.Select(p => p.ToPersonResponse()).ToList();

        _mockPersonsRepository.Setup(t => t.GetFilteredPersons(
            It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
        _mockPersonsRepository.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
        
        // Act
        List<PersonResponse> actualPersons = 
            await _personService.GetFilteredPersons(nameof(Person.Name), "");
        foreach (PersonResponse person in actualPersons)
        {
            _outputHelper.WriteLine(person.ToString());
        }
        
        // Assert
        actualPersons.Should().BeEquivalentTo(expectedPersons);

    }

    // Add few countries, persons and search based on person Name with search string
    // must return appropriate persons !case-insensitive
    [Fact]
    public async Task GetFilteredPersons_ProperText_ToBeSuccessful()
    {
        // Arrange
        List<Person> persons = [
            _fixture.Build<Person>()
                .With(p => p.Email, "example_1@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_2@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Email, "example_3@example.com")
                .With(p => p.Country, null as Country).Create()
        ];
        List<PersonResponse> expectedPersons = persons.Select(p => p.ToPersonResponse()).ToList();

        _mockPersonsRepository.Setup(t => t.GetFilteredPersons(
            It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync([persons[1]]);
        
        // Act
        List<PersonResponse> actualPersons = 
            await _personService.GetFilteredPersons(nameof(Person.Email), "2");
        
        _outputHelper.WriteLine("\n\nActual: ");
        foreach (PersonResponse person in actualPersons)
            _outputHelper.WriteLine(person.ToString());
        
        
        // Assert
        actualPersons.Should().OnlyContain(temp => 
            temp.Name.Contains("2", StringComparison.OrdinalIgnoreCase));
    }
    
    #endregion

    #region GetSortedPersons()

    // Sort on person Name with descending order
    [Fact]
    public async Task GetSortedPersons_Descending()
    {
        // Arrange
        List<Person> persons = [
            _fixture.Build<Person>()
                .With(p => p.Name, "example3")
                .With(p => p.Email, "example_1@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Name, "example1")
                .With(p => p.Email, "example_2@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Name, "example2")
                .With(p => p.Email, "example_3@example.com")
                .With(p => p.Country, null as Country).Create()
        ];
        List<PersonResponse> personResponses = persons.Select(p => 
            p.ToPersonResponse()).ToList();
        
        // Act
        var actualPersons = await _personService.GetSortedPersons(personResponses,
            nameof(Person.Name), SortOrderEnum.Descending);

        // Assert
        actualPersons.Should().BeInDescendingOrder(temp => temp.Name);
    }
    
    // Sort on person Name with ascending order
    [Fact]
    public async Task GetSortedPersons_Ascending()
    {
        // Arrange
        List<Person> persons = [
            _fixture.Build<Person>()
                .With(p => p.Name, "example3")
                .With(p => p.Email, "example_1@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Name, "example1")
                .With(p => p.Email, "example_2@example.com")
                .With(p => p.Country, null as Country).Create(),
            _fixture.Build<Person>()
                .With(p => p.Name, "example2")
                .With(p => p.Email, "example_3@example.com")
                .With(p => p.Country, null as Country).Create()
        ];
        List<PersonResponse> personResponses = persons.Select(p => 
            p.ToPersonResponse()).ToList();
        
        // Act
        var actualPersons = await _personService.GetSortedPersons(personResponses,
            nameof(Person.Name), SortOrderEnum.Ascending);

        // Assert
        actualPersons.Should().BeInAscendingOrder(p => p.Name);

    }

    #endregion

    #region UpdatePerson()

    // When supplied null PersonUpdateRequest throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPersonUpdateRequest_ThrowArgumentNullException()
    {
        // Arrange
        PersonUpdateRequest? personUpdate = null;
        
        // Act
        Func<Task> action = async () =>
            await _personService.UpdatePerson(personUpdate);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    // When supplied invalid Person Id throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonId_ThrowArgumentException()
    {
        // Arrange
        PersonUpdateRequest personUpdate = new PersonUpdateRequest() 
            { PersonId = Guid.NewGuid() };

        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync((Person?)null);
        
        // Act
        Func<Task> action = async () =>
            await _personService.UpdatePerson(personUpdate);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    // When the Person Name is null
    [Fact]
    public async Task UpdatePerson_NullPersonName_ThrowArgumentException()
    {
        // Arrange
        PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.Name, null as string)
            .With(p => p.Email, "example_1@example.com").Create();
        Person person = personUpdateRequest.ToPerson();
        
        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        _mockPersonsRepository.Setup(t => t.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);
        
        // Act
        Func<Task> action = async () =>
            await _personService.UpdatePerson(personUpdateRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    // When proper PersonUpdateRequest
    [Fact]
    public async Task UpdatePerson_ProperRequest_ToBeSuccessful()
    {
        // Arrange
        PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.Email, "example@example.com") .Create();
        Person person = personUpdateRequest.ToPerson();
        PersonResponse expectedPersonResponse = person.ToPersonResponse();

        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        _mockPersonsRepository.Setup(t => t.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);
        
        // Act
        var actualPersonResponse = await _personService.UpdatePerson(personUpdateRequest);
        
        // Assert
        actualPersonResponse.Should().Be(expectedPersonResponse);
    }

    #endregion

    #region DeletePerson()

    // If you supply null PersonId
    [Fact]
    public async Task DeletePerson_NullPersonId_ThrowArgumentNullException()
    {
        Func<Task> action = async () =>
            await _personService.DeletePerson(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    // if you supply an invalid PersonId return false
    [Fact]
    public async Task DeletePerson_InvalidPersonId_ToBeFalse()
    {
        bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

        isDeleted.Should().BeFalse();
    }
    
    // If you supply a valid PersonId return true
    [Fact]
    public async Task DeletePerson_ValidPersonId_ToBeTrue()
    {
        Person person = _fixture.Build<Person>()
            .With(p => p.PersonId, Guid.NewGuid)
            .With(p => p.Email, "example_1@example.com")
            .With(p => p.Country, null as Country).Create();

        _mockPersonsRepository.Setup(t => t.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        _mockPersonsRepository.Setup(t => t.DeletePersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        
        bool isDeleted = await _personService.DeletePerson(person.PersonId);
        
        isDeleted.Should().BeTrue();
    }

    #endregion
}