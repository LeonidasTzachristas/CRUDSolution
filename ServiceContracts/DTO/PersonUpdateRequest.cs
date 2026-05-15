using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents the DTO class for updating the Person entity
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "{0} cannot be empty")]
    public Guid PersonId { get; set; }
    [Required(ErrorMessage = "{0} cannot be empty")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "{0} cannot be empty")]
    [EmailAddress(ErrorMessage = "{0} should be a valid email")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// Converts the current object from type PersonUpdateRequest
    /// to a new object of type Person
    /// </summary>
    /// <returns>The Person object</returns>
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
}