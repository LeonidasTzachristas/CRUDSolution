using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts;

/// <summary>
/// Represents business logic to manipulate Person entity
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Adds a new Person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">The PersonAddRequest object</param>
    /// <returns>The PersonResponse object from the Person object added to list</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Returns the Person object based on the given Guid
    /// </summary>
    /// <param name="personId">The Guid of the Person to search</param>
    /// <returns>Returns the matching Person as object type PersonResponse</returns>
    PersonResponse? GetPersonById(Guid? personId);

    /// <summary>
    /// Get all the existing Person from the List of Persons
    /// </summary>
    /// <returns>Returns the list of persons</returns>
    List<PersonResponse> GetAllPersons();

    /// <summary>
    /// Return all the person objects that match the search field and search string
    /// </summary>
    /// <param name="searchBy">The field to search</param>
    /// <param name="searchString">The Search string</param>
    /// <returns>The list of all person objects that match</returns>
    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
    
    /// <summary>
    /// Sorts the list of PersonResponse objects
    /// </summary>
    /// <param name="allPersons">The List of PersonResponse to sort</param>
    /// <param name="sortBy">The column/ property on which the sort is performed</param>
    /// <param name="sortOrder">Enum for ascending or descending order of the sort</param>
    /// <returns>The sorted List of PersonResponse</returns>
    List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons,
        string sortBy, SortOrderEnum sortOrder);

    /// <summary>
    /// Updates the specified person details based on provided Person Id
    /// </summary>
    /// <param name="personUpdateRequest">Person details to update</param>
    /// <returns>Returns the PersonResponse object updated</returns>
    PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    /// Deletes the Person from the list with the specified person Id
    /// </summary>
    /// <param name="personId">The Guid Id of the person to delete</param>
    /// <returns>Returns true if the person deleted correctly otherwise false</returns>
    bool DeletePerson(Guid? personId);
}