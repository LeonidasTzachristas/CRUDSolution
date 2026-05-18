using Bogus;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Person = Entities.Person;

namespace Services;

public class BogusService
{
    public CountryAddRequest GenerateCountryAddRequest()
    {
        var countryFaker = new Faker<CountryAddRequest>()
            .RuleFor(c => c.CountryName, f => f.Address.Country());
        return countryFaker.Generate();
    }

    public PersonAddRequest GeneratePersonAddRequest()
    {
        var personFaker = new Faker<PersonAddRequest>()
            .RuleFor(p => p.Name, f => f.Name.FirstName())
            .RuleFor(p => p.Email, (f, p) => f.Internet.ExampleEmail(p.Name))
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(30))
            .RuleFor(p => p.Gender, f => f.PickRandom<GenderOptions>())
            .RuleFor(p => p.Address, f => f.Address.StreetAddress());
        var personAddRequest = personFaker.Generate();
        personAddRequest.ReceiveNewsLetters = true;
        return personAddRequest;
    }

    public Country GenerateCountry()
    {
        var countryFaker = new Faker<Country>()
            .RuleFor(c => c.CountryId, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Address.Country());
        return countryFaker.Generate();
    }

    public Person GeneratePerson()
    {
        var genders = new[] { "Male", "Female", "Other" };
        var personFaker = new Faker<Person>()
            .RuleFor(p => p.PersonId, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Name.FirstName())
            .RuleFor(p => p.Email, (f, p) => f.Internet.ExampleEmail(p.Name))
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(30))
            .RuleFor(p => p.Gender, f => f.PickRandom(genders))
            .RuleFor(p => p.Address, f => f.Address.StreetAddress())
            .RuleFor(p => p.ReceiveNewsLetters, f => f.Random.Bool());
        return personFaker.Generate();
    }
}