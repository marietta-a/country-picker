using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CountryPicker.Blazor;

public partial class CityPicker
{
    [Inject]
    private CountryPickerService CountryPickerService { get; set; } = default!;

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

    /// <summary>
    /// Whether to show the city icon (🏙️).
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Visual theme of the component (Light, Dark, Blue, Forest).
    /// </summary>
    [Parameter]
    public PickerTheme Theme { get; set; } = PickerTheme.Light;

    /// <summary>
    /// Border radius roundness of the component.
    /// </summary>
    [Parameter]
    public PickerRoundness Roundness { get; set; } = PickerRoundness.Medium;

    /// <summary>
    /// Custom CSS class to apply to the root container.
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Custom inline styles to apply to the root container.
    /// </summary>
    [Parameter]
    public string Style { get; set; } = string.Empty;

    private string GetInlineStyles()
    {
        var styles = new List<string>();

        // Set Radius
        var radius = Roundness switch
        {
            PickerRoundness.None => "0px",
            PickerRoundness.Small => "4px",
            PickerRoundness.Medium => "8px",
            PickerRoundness.Large => "12px",
            PickerRoundness.Full => "9999px",
            _ => "8px"
        };
        styles.Add($"--cp-radius: {radius}");

        // Set Colors depending on theme
        switch (Theme)
        {
            case PickerTheme.Dark:
                styles.Add("--cp-bg: #1e1e1e");
                styles.Add("--cp-trigger-bg: #2d2d2d");
                styles.Add("--cp-text: #f5f5f5");
                styles.Add("--cp-text-muted: #888888");
                styles.Add("--cp-border: #444444");
                styles.Add("--cp-border-hover: #555555");
                styles.Add("--cp-accent: #0078d4");
                styles.Add("--cp-accent-rgba: rgba(0, 120, 212, 0.25)");
                styles.Add("--cp-dropdown-bg: #1e1e1e");
                styles.Add("--cp-dropdown-border: #333333");
                styles.Add("--cp-dropdown-shadow: rgba(0, 0, 0, 0.5)");
                styles.Add("--cp-item-hover-bg: #2d2d2d");
                styles.Add("--cp-item-hover-text: #ffffff");
                styles.Add("--cp-item-selected-bg: #263c50");
                styles.Add("--cp-item-selected-text: #5ea6e6");
                styles.Add("--cp-item-selected-code: #89c4f4");
                styles.Add("--cp-search-bg: #252526");
                break;

            case PickerTheme.Blue:
                styles.Add("--cp-bg: #ffffff");
                styles.Add("--cp-trigger-bg: #f4f8fa");
                styles.Add("--cp-text: #1b365d");
                styles.Add("--cp-text-muted: #5c768d");
                styles.Add("--cp-border: #b0c4de");
                styles.Add("--cp-border-hover: #4682b4");
                styles.Add("--cp-accent: #1e90ff");
                styles.Add("--cp-accent-rgba: rgba(30, 144, 255, 0.2)");
                styles.Add("--cp-dropdown-bg: #ffffff");
                styles.Add("--cp-dropdown-border: #b0c4de");
                styles.Add("--cp-dropdown-shadow: rgba(27, 54, 93, 0.15)");
                styles.Add("--cp-item-hover-bg: #f0f8ff");
                styles.Add("--cp-item-hover-text: #1b365d");
                styles.Add("--cp-item-selected-bg: #e6f2ff");
                styles.Add("--cp-item-selected-text: #1e90ff");
                styles.Add("--cp-item-selected-code: #87cefa");
                styles.Add("--cp-search-bg: #f0f8ff");
                break;

            case PickerTheme.Forest:
                styles.Add("--cp-bg: #ffffff");
                styles.Add("--cp-trigger-bg: #fdfefd");
                styles.Add("--cp-text: #1e3f20");
                styles.Add("--cp-text-muted: #5e7d5f");
                styles.Add("--cp-border: #c8d6ca");
                styles.Add("--cp-border-hover: #557c56");
                styles.Add("--cp-accent: #2e7d32");
                styles.Add("--cp-accent-rgba: rgba(46, 125, 50, 0.2)");
                styles.Add("--cp-dropdown-bg: #ffffff");
                styles.Add("--cp-dropdown-border: #c8d6ca");
                styles.Add("--cp-dropdown-shadow: rgba(30, 63, 32, 0.15)");
                styles.Add("--cp-item-hover-bg: #f1f8f3");
                styles.Add("--cp-item-hover-text: #1e3f20");
                styles.Add("--cp-item-selected-bg: #e8f5e9");
                styles.Add("--cp-item-selected-text: #2e7d32");
                styles.Add("--cp-item-selected-code: #a5d6a7");
                styles.Add("--cp-search-bg: #f1f8f3");
                break;

            case PickerTheme.Light:
            default:
                break;
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            styles.Add(Style);
        }

        return string.Join("; ", styles);
    }

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
        var source = CountryPickerService.GetCities(CountryCode, StateId);

        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var query = _searchQuery.Trim();
            source = source.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return source;
    }
}
