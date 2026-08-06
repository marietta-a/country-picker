using System;
using System.Collections.Generic;
using System.Linq;

namespace CountryPicker.Blazor;

/// <summary>
/// Service providing towns and cities for countries, dynamically loading them
/// from the rich, hierarchical dataset in CountryService.
/// </summary>
public class LocationService
{
    private readonly CountryService _countryService;

    public LocationService(CountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Gets states for a given 2-letter country code.
    /// </summary>
    public IEnumerable<State> GetStates(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) 
            return Enumerable.Empty<State>();
        
        var country = _countryService.GetByCountryCode(countryCode);
        if (country == null) 
            return Enumerable.Empty<State>();

        return country.States.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets cities for a given country code and state ID.
    /// </summary>
    public IEnumerable<City> GetCities(string? countryCode, int? stateId)
    {
        if (string.IsNullOrWhiteSpace(countryCode) || stateId == null) 
            return Enumerable.Empty<City>();

        var country = _countryService.GetByCountryCode(countryCode);
        if (country == null) 
            return Enumerable.Empty<City>();

        var state = country.States.FirstOrDefault(s => s.Id == stateId);
        if (state == null) 
            return Enumerable.Empty<City>();

        return state.Cities.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);
    }
}
