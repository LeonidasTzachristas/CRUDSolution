using Entities;

namespace Services;

public class DbService
{
    private readonly BogusService _bogusService;

    public DbService(BogusService bogusService)
    {
        _bogusService = bogusService;
        for (int i = 0; i < 10; i++)
        {
            Country c = _bogusService.GenerateCountry();
            Person p = _bogusService.GeneratePerson();
            p.CountryId = c.CountryId;
            Countries.Add(c);
            Persons.Add(p);
        }
    }

    public List<Country> Countries { get; set; } = [];
    public List<Person> Persons { get; set; } = [];
}