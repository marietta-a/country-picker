using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CountryPicker.Blazor;

public partial class CountryPicker
{
    [Inject]
    private CountryService CountryService { get; set; } = default!;

    private bool IsOpen { get; set; }
    private string _searchQuery = string.Empty;
    private ElementReference _searchInputRef;
    private bool _shouldFocusSearch;

    /// <summary>
    /// Current selected country. Supports two-way binding (@bind-Value).
    /// </summary>
    [Parameter]
    public Country? Value { get; set; }

    /// <summary>
    /// Event triggered when the selected country changes.
    /// </summary>
    [Parameter]
    public EventCallback<Country?> ValueChanged { get; set; }

    /// <summary>
    /// Event triggered when a country is selected, matching Flutter's onSelect.
    /// </summary>
    [Parameter]
    public EventCallback<Country> OnSelected { get; set; }

    /// <summary>
    /// Optional list of 2-letter country codes (e.g., ["US", "GB"]) to pin at the top.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? FavoriteCountries { get; set; }

    /// <summary>
    /// Optional list of 2-letter country codes to exclude/hide from selection.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? ExcludeCountries { get; set; }

    /// <summary>
    /// Optional list of 2-letter country codes to display exclusively.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? CountryFilter { get; set; }

    /// <summary>
    /// Whether to display dialing phone codes.
    /// </summary>
    [Parameter]
    public bool ShowPhoneCode { get; set; }

    /// <summary>
    /// Whether to show the search input field.
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Placeholder text for the search input.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search country...";

    /// <summary>
    /// Default trigger button placeholder text when nothing is selected.
    /// </summary>
    [Parameter]
    public string ButtonPlaceholder { get; set; } = "Select country";

    /// <summary>
    /// Whether the dropdown is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        // If Value is null, we can check if there's a default we should pre-select, or leave blank.
    }

    private void ToggleDropdown()
    {
        if (Disabled) return;
        
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
                // Soft ignore if element is not ready or disposed
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

    private async Task SelectCountry(Country country)
    {
        Value = country;
        await ValueChanged.InvokeAsync(country);
        await OnSelected.InvokeAsync(country);
        CloseDropdown();
    }

    private IEnumerable<Country> GetFilteredCountries()
    {
        var source = CountryService.GetAll();

        // 1. Apply exclusions
        if (ExcludeCountries?.Any() == true)
        {
            source = source.Where(c => !ExcludeCountries.Contains(c.CountryCode, StringComparer.OrdinalIgnoreCase));
        }

        // 2. Apply explicit country filters
        if (CountryFilter?.Any() == true)
        {
            source = source.Where(c => CountryFilter.Contains(c.CountryCode, StringComparer.OrdinalIgnoreCase));
        }

        // 3. Apply search query
        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var query = _searchQuery.Trim();
            source = source.Where(c => 
                c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.CountryCode.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneCode.Contains(query, StringComparison.OrdinalIgnoreCase)
            );
        }

        var results = source.ToList();

        // 4. Pin favorites to top (only when NOT searching, to maintain search order relevance)
        if (string.IsNullOrWhiteSpace(_searchQuery) && FavoriteCountries?.Any() == true)
        {
            var favList = new List<Country>();
            var otherList = new List<Country>();

            // Group favorites in their defined priority order
            foreach (var favCode in FavoriteCountries)
            {
                var match = results.FirstOrDefault(c => c.CountryCode.Equals(favCode, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    favList.Add(match);
                }
            }

            // Group all other countries
            foreach (var country in results)
            {
                if (!FavoriteCountries.Contains(country.CountryCode, StringComparer.OrdinalIgnoreCase))
                {
                    otherList.Add(country);
                }
            }

            return favList.Concat(otherList);
        }

        return results;
    }
}
