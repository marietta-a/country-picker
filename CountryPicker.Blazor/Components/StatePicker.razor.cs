using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CountryPicker.Blazor;

public partial class StatePicker
{
    [Inject]
    private LocationService LocationService { get; set; } = default!;

    private bool IsOpen { get; set; }
    private string _searchQuery = string.Empty;
    private ElementReference _searchInputRef;
    private bool _shouldFocusSearch;
    private string? _previousCountryCode;

    /// <summary>
    /// Current selected state. Supports two-way binding (@bind-Value).
    /// </summary>
    [Parameter]
    public State? Value { get; set; }

    /// <summary>
    /// Event triggered when the selected state changes.
    /// </summary>
    [Parameter]
    public EventCallback<State?> ValueChanged { get; set; }

    /// <summary>
    /// Event triggered when a state is selected.
    /// </summary>
    [Parameter]
    public EventCallback<State> OnSelected { get; set; }

    /// <summary>
    /// Country code to load states for.
    /// </summary>
    [Parameter]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Whether to show the search input field.
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Placeholder text for the search input.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Search state...";

    /// <summary>
    /// Default trigger button placeholder text when nothing is selected.
    /// </summary>
    [Parameter]
    public string ButtonPlaceholder { get; set; } = "Select state";

    /// <summary>
    /// Whether the dropdown is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    protected override void OnParametersSet()
    {
        if (CountryCode != _previousCountryCode)
        {
            _previousCountryCode = CountryCode;
            if (Value != null)
            {
                Value = null;
                ValueChanged.InvokeAsync(null);
            }
        }
    }

    private void ToggleDropdown()
    {
        if (Disabled || string.IsNullOrEmpty(CountryCode)) return;
        
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

    private async Task SelectState(State state)
    {
        Value = state;
        await ValueChanged.InvokeAsync(state);
        await OnSelected.InvokeAsync(state);
        CloseDropdown();
    }

    private IEnumerable<State> GetFilteredStates()
    {
        var source = LocationService.GetStates(CountryCode);

        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            var query = _searchQuery.Trim();
            source = source.Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return source;
    }
}
