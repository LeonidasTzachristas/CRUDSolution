using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TINUpdated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
ALTER PROCEDURE [dbo].[GetAllPersons] 
AS BEGIN 
    SELECT PersonId, Name, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsLetters, TaxIdentificationNumber
    FROM [dbo].[Persons] 
END";
            migrationBuilder.Sql(sp_GetAllPersons);
            
            string sp_InsertPerson = @"
ALTER PROCEDURE [dbo].[InsertPerson]
(@PersonId uniqueidentifier, @Name nvarchar(40), @Email nvarchar(50), @DateOfBirth datetime2, 
@Gender nvarchar(10), @CountryId uniqueidentifier, @Address nvarchar(200), @ReceiveNewsLetters bit, @TaxIdentificationNumber varchar(8))
AS BEGIN
    INSERT INTO [dbo].[Persons](PersonId, Name, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsLetters, TaxIdentificationNumber)
    VALUES (@PersonId, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters, @TaxIdentificationNumber)
END
";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"DROP PROCEDURE [dbo].[GetAllPersons]";
            migrationBuilder.Sql(sp_GetAllPersons);
            
            string sp_InsertPerson = @"DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
