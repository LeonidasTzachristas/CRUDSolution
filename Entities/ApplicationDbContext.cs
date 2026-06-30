using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<Person> Persons => Set<Person>();
    
    public virtual DbSet<Country> Countries => Set<Country>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
        
        // Fluent API
        modelBuilder.Entity<Person>().Property(p => p.TIN)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABCD1234");

        // modelBuilder.Entity<Person>().HasIndex(p => p.TIN).IsUnique();
        
        // Table Relations
        // modelBuilder.Entity<Person>(entity =>
        // {
        //     entity.HasOne<Country>(c => c.Country)
        //         .WithMany(p => p.Persons)
        //         .HasForeignKey(p => p.CountryId);
        // });
        // modelBuilder.Entity<Person>()
        //     .HasOne<Country>(p => p.Country)
        //     .WithMany(c => c.Persons)
        //     .HasForeignKey(p => p.CountryId);
    }
    
    public List<Person> sp_GetAllPersons()
    {
        return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }

    public async Task<int> sp_InsertPersonAsync(Person person)
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
            new("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            new(@"TaxIdentificationNumber", person.TIN)
        ];
        return await Database.ExecuteSqlRawAsync("EXECUTE [dbo].[InsertPerson] @PersonId, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters, @TaxIdentificationNumber", parameters);
    }
}