using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class CountriesRepository : ICountriesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CountriesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    
    public async Task<Country> AddCountry(Country country)
    {
        var addedCountryawait = await _dbContext.Countries.AddAsync(country);
        await _dbContext.SaveChangesAsync();
        return addedCountryawait.Entity;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await _dbContext.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountryByCountryId(Guid countryId)
    {
        return await _dbContext.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
    }

    public async Task<Country?> GetCountryByCountryName(string countryName)
    {
        return await _dbContext.Countries.FirstOrDefaultAsync(c => c.Equals(countryName));
    }
}