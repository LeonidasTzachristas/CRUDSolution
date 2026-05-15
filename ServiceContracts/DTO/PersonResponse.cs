using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public record PersonResponse
{
    public Guid PersonId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }
    
    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonId = PersonId,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender,
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}

public static class PersonExtensions
{
    /// <summary>
    /// Extension method of Person domain model to convert it to PersonResponse record
    /// </summary>
    /// <param name="person">The Person object to convert</param>
    /// <returns>Returns the converted PersonResponse object</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            Name = person.Name,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender switch
            {
                "Male" => GenderOptions.Male,
                "Female" => GenderOptions.Female,
                _ => GenderOptions.Other
            },
            CountryId = person.CountryId,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Age = person.DateOfBirth != null 
                ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                : null
        };
    }
}