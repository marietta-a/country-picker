using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CountryPicker.Blazor;

/// <summary>
/// Represents a country with ISO code, flag emoji, and its hierarchical states/provinces.
/// </summary>
public class Country
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("emoji")]
    public string Emoji { get; set; } = string.Empty;

    [JsonPropertyName("emojiU")]
    public string EmojiU { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public List<State> States { get; set; } = [];

    /// <summary>
    /// Two-letter ISO country code (ISO 3166-1 alpha-2, e.g., "US", "GB", "GR").
    /// Derived dynamically from the emoji Unicode codepoints.
    /// </summary>
    [JsonIgnore]
    public string CountryCode => GetCountryCodeFromEmojiU(EmojiU);

    /// <summary>
    /// Visual flag emoji representation of the country.
    /// </summary>
    [JsonIgnore]
    public string FlagEmoji => Emoji;

    /// <summary>
    /// International country dialing code (e.164 country code, e.g., "1", "44", "30").
    /// </summary>
    [JsonIgnore]
    public string PhoneCode => GetPhoneCode(CountryCode);

    /// <summary>
    /// An example of a valid national phone number for this country.
    /// </summary>
    [JsonIgnore]
    public string Example => GetExamplePhone(CountryCode);

    /// <summary>
    /// Full standard display name of the country.
    /// </summary>
    [JsonIgnore]
    public string DisplayName => $"{Name} ({CountryCode})";

    /// <summary>
    /// Converts Regional Indicator codepoints in "emojiU" string (e.g., "U+1F1E6 U+1F1EB") into a 2-letter ISO country code.
    /// </summary>
    public static string GetCountryCodeFromEmojiU(string emojiU)
    {
        if (string.IsNullOrWhiteSpace(emojiU)) return string.Empty;
        var parts = emojiU.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return string.Empty;

        try
        {
            int code1 = Convert.ToInt32(parts[0].Replace("U+", ""), 16);
            int code2 = Convert.ToInt32(parts[1].Replace("U+", ""), 16);

            char char1 = (char)(code1 - 0x1F1E6 + 'A');
            char char2 = (char)(code2 - 0x1F1E6 + 'A');

            return $"{char1}{char2}";
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GetPhoneCode(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode)) return "1";
        return CountryPhoneCodes.Codes.TryGetValue(countryCode.ToUpperInvariant(), out var data) ? data.PhoneCode : "1";
    }

    private static string GetExamplePhone(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode)) return "202-555-0199";
        return ExamplePhoneMap.TryGetValue(countryCode.ToUpperInvariant(), out var example) ? example : "202-555-0199";
    }

    private static readonly Dictionary<string, string> ExamplePhoneMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "US", "202-555-0199" }, { "CA", "416-555-0199" }, { "GB", "7700 900077" }, 
        { "AU", "0491 570 156" }, { "GR", "691 234 5678" }, { "FR", "06 12 34 56 78" }, 
        { "DE", "0151 2345678" }, { "IN", "98123 45678" }, { "CN", "138 1234 5678" }, 
        { "JP", "090-1234-5678" }
    };
}

/// <summary>
/// Represents a state or province within a country.
/// </summary>
public class State
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    [JsonPropertyName("country_id")]
    public int CountryId { get; set; }

    [JsonPropertyName("city")]
    public List<City> Cities { get; set; } = [];
}

/// <summary>
/// Represents a city within a state.
/// </summary>
public class City
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    [JsonPropertyName("state_id")]
    public int StateId { get; set; }
}
