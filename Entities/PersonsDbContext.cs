using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class PersonsDbContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();
    
    public DbSet<Country> Countries => Set<Country>();

    public PersonsDbContext(DbContextOptions<PersonsDbContext> options)
        : base(options)
    {  
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");
        
        
        // Seeding to Countries
        var countries = JsonSerializer.Deserialize<List<Country>>
            (File.ReadAllText("C:\\Users\\leoni\\RiderProjects\\CRUDSolution\\Entities\\DbSeeds\\countries.json"));
        if (countries is null)
            Console.WriteLine("WTF Bro");
        foreach (Country country in countries)
        {
            modelBuilder.Entity<Country>().HasData(country);
        }
        
        // Seeding to Persons
        var persons = JsonSerializer.Deserialize<List<Person>>
            (File.ReadAllText("C:\\Users\\leoni\\RiderProjects\\CRUDSolution\\Entities\\DbSeeds\\persons.json"));
        foreach (Person person in persons)
        {
            modelBuilder.Entity<Person>().HasData(person);
        }
    }
    
    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }

    public int sp_InsertPerson(Person person)
    {
        SqlParameter[] parameters =
        [
            new("@PersonId", person.PersonId),
            new("@Name", person.Name),
            new("@Email", person.Email),
            new("@DateOfBirth", person.DateOfBirth),
            new("@Gender", person.Gender),
            new("@CountryId", person.CountryId),
            new("@Address", person.Address),
            new("@ReceiveNewsLetters", person.ReceiveNewsLetters)
        ];
        return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters", parameters);
    }
}