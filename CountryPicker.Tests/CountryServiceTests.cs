using Xunit;
using CountryPicker.Blazor;

namespace CountryPicker.Tests;

public class CountryServiceTests
{
    [Fact]
    public void GetAll_ReturnsLoadedCountriesDataset()
    {
        // Arrange
        var service = new CountryService();

        // Act
        var countries = service.GetAll().ToList();

        // Assert
        Assert.NotNull(countries);
        Assert.NotEmpty(countries);
        Assert.True(countries.Count > 200, $"Expected dataset to contain over 200 countries, but found {countries.Count}");
    }

    [Fact]
    public void CountryFields_AreCorrectlyParsed()
    {
        // Arrange
        var service = new CountryService();

        // Act & Assert for US
        var us = service.GetByCountryCode("US");
        Assert.NotNull(us);
        Assert.Equal("United States", us.Name);
        Assert.Equal("🇺🇸", us.Emoji); // Emoji verification
        Assert.Equal("US", us.CountryCode); // CountryCode verification
        Assert.Equal("1", us.PhoneCode); // PhoneCode verification
        Assert.NotEmpty(us.States); // Hierarchical data check

        // Act & Assert for GR (Greece)
        var gr = service.GetByCountryCode("GR");
        Assert.NotNull(gr);
        Assert.Equal("Greece", gr.Name);
        Assert.Equal("🇬🇷", gr.Emoji);
        Assert.Equal("GR", gr.CountryCode);
        Assert.Equal("30", gr.PhoneCode);
    }

    [Theory]
    [InlineData("US", "United States")]
    [InlineData("us", "United States")] // Case insensitivity test
    [InlineData("GB", "United Kingdom")]
    [InlineData("gb", "United Kingdom")]
    [InlineData("GR", "Greece")]
    public void GetByCountryCode_ReturnsExpectedCountry_CaseInsensitive(string code, string expectedName)
    {
        // Arrange
        var service = new CountryService();

        // Act
        var country = service.GetByCountryCode(code);

        // Assert
        Assert.NotNull(country);
        Assert.Equal(expectedName, country.Name);
        Assert.Equal(code.ToUpperInvariant(), country.CountryCode);
    }

    [Fact]
    public void GetByCountryCode_ReturnsNullForInvalidOrEmptyCode()
    {
        // Arrange
        var service = new CountryService();

        // Act & Assert
        Assert.Null(service.GetByCountryCode("ZZ"));
        Assert.Null(service.GetByCountryCode(""));
        Assert.Null(service.GetByCountryCode("   "));
    }

    [Fact]
    public void GetByPhoneCode_ReturnsMatchingCountries()
    {
        // Arrange
        var service = new CountryService();

        // Act
        var matches = service.GetByPhoneCode("1").ToList();

        // Assert
        Assert.NotEmpty(matches);
        Assert.Contains(matches, c => c.CountryCode == "US");
        Assert.Contains(matches, c => c.CountryCode == "CA");
    }

    [Fact]
    public void LocationService_ReturnsStatesAndCitiesCorrectly()
    {
        // Arrange
        var countryService = new CountryService();
        var locationService = new LocationService(countryService);

        // Act - US States
        var usStates = locationService.GetStates("US").ToList();
        
        // Assert
        Assert.NotEmpty(usStates);
        var California = usStates.FirstOrDefault(s => s.Name == "California");
        Assert.NotNull(California);

        // Act - California Cities
        var caliCities = locationService.GetCities("US", California.Id).ToList();

        // Assert
        Assert.NotEmpty(caliCities);
        Assert.Contains(caliCities, c => c.Name == "Los Angeles");
    }
}
