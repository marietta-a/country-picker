using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CountryPicker.Blazor;

public partial class CityPicker
{
    [Inject]
    private LocationService LocationService { get; set; } = default!;

    private bool IsOpen { get; set; }
    private string _searchQuery = string.Empty;
    private ElementReference _searchInputRef;
    private bool _shouldFocusSearch;
    private int? _previousStateId;

    /// <summary>
    /// Current selected city. Supports two-way binding (@bind-Value).
    /// </summary>
    [Parameter]
    public City? Value { get; set; }

    /// <summary>
    /// Event triggered when the selected city changes.
    /// </summary>
    [Parameter]
    public EventCallback<City?> ValueChanged { get; set; }

    /// <summary>
    /// Event triggered when a city is selected.
    /// </summary>
    [Parameter]
    public EventCallback<City> OnSelected { get; set; }

    /// <summary>
    /// Country code for validation and context.
    /// </summary>
    [Parameter]
    public string? CountryCode { get; set; }

    /// <summary>
    /// State ID to load cities for.
    /// </summary>
    [Parameter]
    public int? StateId { get; set; }

    /// <summary>
    /// Whether to show the search input field.
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Placeholder text for the search input.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search city...";

    /// <summary>
    /// Default trigger button placeholder text when nothing is selected.
    /// </summary>
    [Parameter]
    public string ButtonPlaceholder { get; set; } = "Select city";

    /// <summary>
    /// Whether the dropdown is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    protected override void OnParametersSet()
    {
        if (StateId != _previousStateId)
        {
            _previousStateId = StateId;
            if (Value != null)
            {
                Value = null;
                ValueChanged.InvokeAsync(null);
            }
        }
    }

    private void ToggleDropdown()
    {
        if (Disabled || StateId == null) return;
        
        IsOpen = !IsOpen;
        _searchQuery = string.Empty; // Reset search upon toggling
        if (IsOpen)
        {
            _shouldFocusSearch = true;
        }
    }

    private void CloseDropdown()
    {
        IsOpen = false;
        _searchQuery = string.Empty;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocusSearch && ShowSearch && IsOpen)
        {
            _shouldFocusSearch = false;
            try
            {
                await _searchInputRef.FocusAsync();
            }
            catch
            {
                // Soft ignore
            }
        }
    }

    private void OnSearchInput(ChangeEventArgs e)
    {
        _searchQuery = e.Value?.ToString() ?? string.Empty;
    }

    private void ClearSearch()
    {
        _searchQuery = string.Empty;
    }

    private async Task SelectCity(City city)
    {
        Value = city;
        await ValueChanged.InvokeAsync(city);
        await OnSelected.InvokeAsync(city);
        CloseDropdown();
    }

    private IEnumerable<City> GetFilteredCities()
    {
        var source = LocationService.GetCities(CountryCode, StateId);

        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var query = _searchQuery.Trim();
            source = source.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return source;
    }
}
