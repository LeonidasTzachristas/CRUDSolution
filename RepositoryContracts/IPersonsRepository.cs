using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts;

/// <summary>
/// Represents data access logic for managing Person Entity
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    /// Adds a new person object to the data store
    /// </summary>
    /// <param name="person">The Person object to add</param>
    /// <returns>Returns the Person object after adding to the data store</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// Gets all the Persons in the data store
    /// </summary>
    /// <returns>Returns the list of person in the data store</returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    /// Gets the person based on the given person Id
    /// </summary>
    /// <param name="personId">The Guid person Id to search on</param>
    /// <returns>Returns the Person object with the specified person Id or null if not found</returns>
    Task<Person?> GetPersonByPersonId(Guid personId);

    /// <summary>
    /// Returns all person objects based on the given expression
    /// </summary>
    /// <param name="predicate">LINQ expression to check</param>
    /// <returns>Returns all matching persons with specified condition</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

    /// <summary>
    /// Deletes a person object from the data store based on the given person Id
    /// </summary>
    /// <param name="personId">The Guid person Id to delete</param>
    /// <returns>Returns true if the deletion is successful: otherwise false</returns>
    Task<bool> DeletePersonByPersonId(Guid personId);

    /// <summary>
    /// Updates a person object in the data store based on the given person Id
    /// </summary>
    /// <param name="person">Person object to update</param>
    /// <returns>Returns the updated person object</returns>
    Task<Person> UpdatePerson(Person person);
}