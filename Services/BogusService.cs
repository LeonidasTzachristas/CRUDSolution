using Bogus;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Services;

public class BogusService
{
    public CountryAddRequest GenerateCountry()
    {
        var countryFaker = new Faker<CountryAddRequest>()
            .RuleFor(c => c.CountryName, f => f.Address.Country());
        return countryFaker.Generate();
    }

    public PersonAddRequest GeneratePerson()
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
}