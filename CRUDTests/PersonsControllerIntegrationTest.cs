using FluentAssertions;
using Xunit.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CRUDTests;

public class PersonsControllerIntegrationTest 
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public PersonsControllerIntegrationTest
    (CustomWebApplicationFactory webAppFactory, ITestOutputHelper output)
    {
        _output = output;
        _client = webAppFactory.CreateClient();
    }


    #region Index

    [Fact]
    public async Task Index_ToReturnView()
    
    {
        // Arrange
        
        // Act
        HttpResponseMessage httpResponse = await _client.GetAsync("/Persons/Index");
        
        // Assert
        httpResponse.IsSuccessStatusCode.Should().BeTrue();
        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        var html = new HtmlDocument();
        html.LoadHtml(responseBody);
        var document = html.DocumentNode;

        _output.WriteLine(responseBody);
        document.QuerySelectorAll("table.persons").Should().NotBeNull();
    }

    #endregion
    
}