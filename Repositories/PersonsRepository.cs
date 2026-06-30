using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PersonsRepository> _logger;

    public PersonsRepository(ApplicationDbContext dbContext, 
        ILogger<PersonsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Person> AddPerson(Person person)
    {
        var addedPerson = await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();
        return addedPerson.Entity;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _dbContext.Persons.AsNoTracking().Include("Country").ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonId(Guid personId)
    {
        return await _dbContext.Persons.AsNoTracking().Include("Country")
            .FirstOrDefaultAsync(p => p.PersonId == personId);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        // Logging
        _logger.LogInformation("GetFilteredPersons of PersonsRepository");
        
        return await _dbContext.Persons.AsNoTracking().Include("Country")
            .Where(predicate).ToListAsync();
    }

    public async Task<bool> DeletePersonByPersonId(Guid personId)
    {
        _dbContext.Persons.RemoveRange(_dbContext.Persons.Where(p => 
            p.PersonId == personId));
        var rowsDeleted = await _dbContext.SaveChangesAsync();
        
        return rowsDeleted > 0;
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person? matchingPerson = await _dbContext.Persons
            .FirstOrDefaultAsync(p => p.PersonId == person.PersonId);

        if (matchingPerson is null)
            return person;

        matchingPerson.Name = person.Name;
        matchingPerson.Email = person.Email;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.Gender = person.Gender;
        matchingPerson.CountryId = person.CountryId;
        matchingPerson.Address = person.Address;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

        var rowsUpdated = await _dbContext.SaveChangesAsync();

        return matchingPerson;
    }
}