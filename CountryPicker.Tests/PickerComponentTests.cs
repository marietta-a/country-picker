using Xunit;
using CountryPicker.Blazor;

namespace CountryPicker.Tests;

public class PickerComponentTests
{
    [Fact]
    public void StatePicker_ShowIcon_DefaultsToTrue()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.StatePicker();

        // Assert
        Assert.True(picker.ShowIcon);
    }

    [Fact]
    public void CityPicker_ShowIcon_DefaultsToTrue()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CityPicker();

        // Assert
        Assert.True(picker.ShowIcon);
    }

    [Fact]
    public void CountryPicker_Theme_DefaultsToLight()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CountryPicker();

        // Assert
        Assert.Equal(PickerTheme.Light, picker.Theme);
    }

    [Fact]
    public void CountryPicker_Roundness_DefaultsToMedium()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CountryPicker();

        // Assert
        Assert.Equal(PickerRoundness.Medium, picker.Roundness);
    }

    [Fact]
    public void CountryPicker_Class_DefaultsToEmpty()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CountryPicker();

        // Assert
        Assert.Equal(string.Empty, picker.Class);
    }

    [Fact]
    public void CountryPicker_Style_DefaultsToEmpty()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CountryPicker();

        // Assert
        Assert.Equal(string.Empty, picker.Style);
    }

    [Fact]
    public void StatePicker_Theme_DefaultsToLight()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.StatePicker();

        // Assert
        Assert.Equal(PickerTheme.Light, picker.Theme);
    }

    [Fact]
    public void StatePicker_Roundness_DefaultsToMedium()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.StatePicker();

        // Assert
        Assert.Equal(PickerRoundness.Medium, picker.Roundness);
    }

    [Fact]
    public void CityPicker_Theme_DefaultsToLight()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CityPicker();

        // Assert
        Assert.Equal(PickerTheme.Light, picker.Theme);
    }

    [Fact]
    public void CityPicker_Roundness_DefaultsToMedium()
    {
        // Arrange & Act
        var picker = new CountryPicker.Blazor.CityPicker();

        // Assert
        Assert.Equal(PickerRoundness.Medium, picker.Roundness);
    }
}
