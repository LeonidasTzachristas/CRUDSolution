using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

/// <summary>
/// Person domain model class
/// </summary>
public class Person
{
    [Key]
    public Guid PersonId { get; set; }
    
    [StringLength(40)]
    public string? Name { get; set; }
    
    [StringLength(50)]
    public string? Email { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(10)]
    public string? Gender { get; set; }
    
    public Guid? CountryId { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    public bool ReceiveNewsLetters { get; set; }

    public string? TIN { get; set; }
    
    // Used as Navigation Property to make relation in EFCore
    [ForeignKey("CountryId")]
    public Country? Country { get; set; }

    public override string ToString() => $"{Name} ({Email}) - {DateOfBirth}";
}