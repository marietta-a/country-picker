using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace CountryPicker.Blazor;

/// <summary>
/// Service to load and manage the full dataset of countries from the embedded JSON resource.
/// </summary>
public class CountryService
{
    private readonly List<Country> _countries = [];

    public CountryService()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            // Note: Resource name structure is Namespace.FileName.br
            using var stream = assembly.GetManifestResourceStream("CountryPicker.Blazor.countries.json.br");
            if (stream == null)
            {
                // Fallback attempt in case namespaces differ slightly
                var resourceNames = assembly.GetManifestResourceNames();
                var match = resourceNames.FirstOrDefault(r => r.EndsWith("countries.json.br", StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    using var fallbackStream = assembly.GetManifestResourceStream(match);
                    if (fallbackStream != null)
                    {
                        LoadFromStream(fallbackStream);
                        return;
                    }
                }
                throw new FileNotFoundException("Could not find the embedded countries.json.br resource in assembly.");
            }

            LoadFromStream(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CountryPicker.Blazor] Error loading embedded country codes: {ex.Message}");
        }
    }

    private void LoadFromStream(Stream stream)
    {
#pragma warning disable CA1416 // BrotliStream is fully supported under WebAssembly (Mono) in Blazor
        using var decompressedStream = new System.IO.Compression.BrotliStream(stream, System.IO.Compression.CompressionMode.Decompress);
#pragma warning restore CA1416
        using var reader = new StreamReader(decompressedStream);
        var json = reader.ReadToEnd();

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        var parsed = JsonSerializer.Deserialize<List<Country>>(json, options);
        if (parsed != null)
        {
            _countries.Clear();
            _countries.AddRange(parsed.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Gets all configured countries, sorted alphabetically by country name.
    /// </summary>
    public IEnumerable<Country> GetAll() => _countries;

    /// <summary>
    /// Finds a country by its 2-letter ISO country code (e.g., "US", "GR"). Case-insensitive.
    /// </summary>
    public Country? GetByCountryCode(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return null;
        return _countries.FirstOrDefault(c => c.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds countries matching an e.164 dial code (e.g., "1" or "355" or "358").
    /// </summary>
    public IEnumerable<Country> GetByPhoneCode(string phoneCode)
    {
        if (string.IsNullOrWhiteSpace(phoneCode)) return [];
        var cleanCode = phoneCode.TrimStart('+').Trim();
        return _countries.Where(c => c.PhoneCode.Equals(cleanCode, StringComparison.OrdinalIgnoreCase));
    }
}
