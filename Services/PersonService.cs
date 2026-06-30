using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonService : IPersonService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonService(IPersonsRepository personsRepository, 
        ILogger<PersonService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }
    
    
    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest? personAddRequest)
    {
        // Check if personAddRequest is null
        if (personAddRequest is null)
            throw new ArgumentNullException();
        
        /* Better use model validation */
        ValidationHelper.ModelValidation(personAddRequest);

        // Convert
        Person person = personAddRequest.ToPerson(Guid.NewGuid());
        person.TIN = "9876ZXCV";

        await _personsRepository.AddPerson(person);
        
        // Return the PersonResponse
        PersonResponse responsePerson = person.ToPersonResponse();

        return responsePerson;
    }

    public async Task<PersonResponse?> GetPersonById(Guid? personId)
    {
        // if (personId == null) return null;
        // var person = await _personsRepository.GetPersonByPersonId(personId.Value);
        // return person?.ToPersonResponse();

        return personId is null ? null 
            : (await _personsRepository.GetPersonByPersonId(personId.Value))
            ?.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        // Logging
        _logger.LogInformation("GetAllPersons of PersonsService");
        
        return (await _personsRepository.GetAllPersons())
            .Select(p => p.ToPersonResponse()).ToList();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        // Logging
        _logger.LogInformation("GetFilteredPersons of PersonsService");
        _logger.LogDebug("SearchBy: {SearchBy} | SearchString: {searchString}", 
            searchBy, searchString);

        List<Person> persons;
        using (Operation.Time("Time for filtered persons"))
        {
            persons = searchBy switch
            {
                // Null reference exception doesn't matter -> they will be translated to SQL

                nameof(PersonResponse.Name) => await _personsRepository
                    .GetFilteredPersons(p => p.Name.Contains(searchString)),

                nameof(PersonResponse.Email) => await _personsRepository
                    .GetFilteredPersons(p => p.Email.Contains(searchString)),

                // TODO Fix this
                nameof(PersonResponse.DateOfBirth) =>
                    await _personsRepository
                        .GetFilteredPersons(p => p.DateOfBirth.Value.ToString("yy-MM-dd")
                            .Contains(searchString)),

                nameof(PersonResponse.Gender) => await _personsRepository
                    .GetFilteredPersons(p => p.Gender.ToString().Contains(searchString)),

                nameof(PersonResponse.Address) => await _personsRepository
                    .GetFilteredPersons(p => p.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };
        }   // End of using Serilog timing

        _diagnosticContext.Set("Persons", persons);
        
        return persons.Select(p => p.ToPersonResponse()).ToList();
    }

    public async Task<List<PersonResponse>> GetSortedPersons(
        List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
    {
        // Logging
        _logger.LogInformation("GetSortedPersons of PersonsService");
        
        if (string.IsNullOrEmpty(sortBy))
            return allPersons;
        List<PersonResponse> sortedPersons = sortOrder switch
        {
            SortOrderEnum.Ascending => sortBy switch
            {
                nameof(PersonResponse.Name) => allPersons.OrderBy(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Email) => allPersons.OrderBy(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.DateOfBirth) => allPersons.OrderBy(p => p.DateOfBirth).ToList(),
                nameof(PersonResponse.Gender) => allPersons.OrderBy(p => p.Gender).ToList(),
                nameof(PersonResponse.Address) => allPersons.OrderBy(p => p.Address,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Age) => allPersons.OrderBy(p => p.Age).ToList(),
                nameof(PersonResponse.Country) => allPersons.OrderBy(p => p.Country, StringComparer
                .OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.ReceiveNewsLetters) => allPersons.OrderBy(p => p.ReceiveNewsLetters).ToList(),
                _ => allPersons
            },
            SortOrderEnum.Descending => sortBy switch
            {
                nameof(PersonResponse.Name) => allPersons.OrderByDescending(p => p.Name,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Email) => allPersons.OrderByDescending(p => p.Email,
                    StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.DateOfBirth) => allPersons.OrderByDescending(p =>
                    p.DateOfBirth).ToList(),
                nameof(PersonResponse.Gender) => allPersons.OrderByDescending(p =>
                    p.Gender).ToList(),
                nameof(PersonResponse.Address) => allPersons.OrderByDescending(p =>
                    p.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.Age) => allPersons.OrderByDescending(p => p.Age).ToList(),
                nameof(PersonResponse.Country) => allPersons.OrderByDescending(p => p.Country, StringComparer
                    .OrdinalIgnoreCase).ToList(),
                nameof(PersonResponse.ReceiveNewsLetters) => allPersons.OrderByDescending(p => p.ReceiveNewsLetters).ToList(),
                _ => allPersons
            },
            _ => allPersons
        };
        
        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest is null)
            throw new ArgumentNullException();
        
        ValidationHelper.ModelValidation(personUpdateRequest);

        Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);

        if (matchingPerson is null)
            throw new ArgumentException("Given person Id does not exist");

        matchingPerson.Name = personUpdateRequest.Name ?? matchingPerson.Name;
        matchingPerson.Email = personUpdateRequest.Email ?? matchingPerson.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth ?? matchingPerson.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString() ?? matchingPerson.Gender;
        matchingPerson.CountryId = personUpdateRequest.CountryId ?? matchingPerson.CountryId;
        matchingPerson.Address = personUpdateRequest.Address ?? matchingPerson.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        await _personsRepository.UpdatePerson(matchingPerson);
        
        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));

        Person? personToDelete = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (personToDelete is null)
            return false;

        return await _personsRepository.DeletePersonByPersonId(personId.Value);
    }

    public async Task<MemoryStream> GetPersonsCsv()
    {
        var persons = (await _personsRepository.GetAllPersons())
            .Select(p => p.ToPersonResponse()).ToList();
        
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);
        
        // CsvWriter csvWriter = new(streamWriter, 
        //     CultureInfo.InvariantCulture, leaveOpen: true);
        // csvWriter.WriteHeader<PersonResponse>();
        // await csvWriter.NextRecordAsync();
        // await csvWriter.WriteRecordsAsync(persons);
        
        CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new(streamWriter, csvConfig);
        
        csvWriter.WriteField(nameof(PersonResponse.Name));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
        await csvWriter.NextRecordAsync();

        foreach (PersonResponse p in persons)
        {
            csvWriter.WriteField(p.Name);
            csvWriter.WriteField(p.Email);
            csvWriter.WriteField(p.DateOfBirth?.ToString("yyyy MMMM dd"));
            csvWriter.WriteField(p.Country);
            csvWriter.WriteField(p.Address);
            csvWriter.WriteField(p.ReceiveNewsLetters);
            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        var persons = (await _personsRepository.GetAllPersons())
            .Select(p => p.ToPersonResponse()).ToList();
        
        MemoryStream memoryStream = new MemoryStream();
        
        ExcelPackage.License.SetNonCommercialPersonal("Leonidas");
        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Date of Birth";
            worksheet.Cells["D1"].Value = "Age";
            worksheet.Cells["E1"].Value = "Gender";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
            worksheet.Cells["H1"].Value = "Receive News Letters";

            using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                headerCells.Style.Font.Bold = true;
            }
            
            int row = 2;
            foreach (PersonResponse person in persons)
            {
                worksheet.Cells[row, 1].Value = person.Name;
                worksheet.Cells[row, 2].Value = person.Email;
                worksheet.Cells[row, 3].Value = person.DateOfBirth;
                worksheet.Cells[row, 4].Value = person.Age;
                worksheet.Cells[row, 5].Value = person.Gender;
                worksheet.Cells[row, 6].Value = person.Country;
                worksheet.Cells[row, 7].Value = person.Address;
                worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                row++;
            }
            
            
            worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}