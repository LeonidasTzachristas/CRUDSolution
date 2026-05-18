using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonAddRequest
{
    [Required(ErrorMessage = "{0} cannot be empty")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "{0} cannot be empty")]
    [EmailAddress(ErrorMessage = "{0} should be a valid email")]
    public string? Email { get; set; }
    
    [Required]
    public DateTime? DateOfBirth { get; set; }
    
    
    [Required]
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object from type PersonAddRequest
    /// to a new object of type Person
    /// </summary>
    /// <returns>The newly created Person</returns>
    public Person ToPerson()
    {
        return new Person()
        {
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
    public Person ToPerson(Guid guid)
    {
        return new Person()
        {
            PersonId = guid,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}