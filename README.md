# CountryPicker.Blazor

A lightweight, high-performance, and beautifully styled **Country, State, and City Picker** component library for **Blazor** (Interactive Server & WebAssembly). 

---

## 🌟 Features

- **Zero-Asset Flag Emojis**: Leverages standard Unicode regional indicator sequences to display native emoji flags. No external image assets or font files required!
- **Searchable Dropdowns**: Built-in, high-speed client-side filtering matching by country name, ISO-3166 codes, or phone dialing codes.
- **Favorites / Grouping**: Pin important or frequently used countries (e.g., `["US", "GB", "GR", "CA", "AU"]`) to a dedicated "Favorites" section at the top of the list.
- **Cascading Geography**: Composed of three complementary components—`CountryPicker`, `StatePicker`, and `CityPicker`—which cascade seamlessly. When a parent country/state changes, child components automatically reset their values.
- **Dialing Codes Support**: Toggle international phone dial codes (e.g., `+1`, `+30`, `+44`) directly in the select triggers and list items.
- **Exclusion & Inclusion Filters**: Specify which countries to explicitly exclude/hide or provide a strict list of allowed country codes.
- **🎨 Custom Visual Themes**: Out-of-the-box support for four rich visual themes: `Light`, `Dark`, `Blue`, and `Forest` built on clean CSS variable cascading.
- **📐 Corner Roundness Options**: Easily configure custom container and select button corner radius shapes: `None`, `Small`, `Medium`, `Large`, or `Full` pills.
- **🗺️ Toggleable Geography Icons**: Hide or display helper map (`🗺️`) and city (`🏙️`) icons in the triggers and dropdown list items.
- **Modern UI & Smooth Animations**: Out-of-the-box CSS isolation ensures beautiful, native-feeling panels with micro-interactions, popover animations, clear buttons, and click-outside dismissal (via full-viewport backdrop overlays).
- **Interactive Focus**: Automatically shifts focus to the search query input as soon as the popover dropdown opens.

---

## 🚀 Getting Started

### 1. Installation

Install the package via the NuGet Package Manager Console:

```bash
dotnet add package CountryPicker.Blazor
```

### 2. Register Services

Register the required data and location services in your application container (usually inside `Program.cs`):

```csharp
using CountryPicker.Blazor;

// Register country/geography services
builder.Services.AddSingleton<CountryService>();
builder.Services.AddSingleton<LocationService>();
```

### 3. Add Imports

Add the namespace to your global imports in `_Imports.razor`:

```razor
@using CountryPicker.Blazor
```

---

## 💻 Usage Examples

### Styled Country Picker (Dark Theme & Large Roundness)

Use the `<CountryPicker>` component with two-way binding on the `@bind-Value` attribute, and custom style enums:

```razor
@page "/simple-picker"
@rendermode InteractiveServer

<h3>Select your Country</h3>

<CountryPicker 
    @bind-Value="SelectedCountry" 
    ShowPhoneCode="true" 
    ShowSearch="true"
    Theme="PickerTheme.Dark"
    Roundness="PickerRoundness.Large"
    Placeholder="Search country records..."
    ButtonPlaceholder="Select your country..." />

@if (SelectedCountry != null)
{
    <p>Selected: <strong>@SelectedCountry.Name</strong> (Code: @SelectedCountry.CountryCode, Phone: +@SelectedCountry.PhoneCode)</p>
}

@code {
    private Country? SelectedCountry { get; set; }
}
```

### Cascading Geography (Country ➔ State ➔ City) with Forest Theme

Implement cascading dropdowns easily by linking parent values to child input parameters. State and City options will load dynamically and clear automatically if parent bindings change.

```razor
@page "/cascading-picker"
@rendermode InteractiveServer

<div class="form-group">
    <label>Country:</label>
    <CountryPicker 
        @bind-Value="ActiveCountry"
        FavoriteCountries='new[] { "US", "GB", "CA" }'
        Theme="PickerTheme.Forest"
        Roundness="PickerRoundness.Medium"
        Placeholder="Search country..."
        ButtonPlaceholder="Select Country" />
</div>

<div class="form-group mt-3">
    <label>State / Province:</label>
    <StatePicker 
        @bind-Value="ActiveState"
        CountryCode="@ActiveCountry?.CountryCode"
        Theme="PickerTheme.Forest"
        Roundness="PickerRoundness.Medium"
        ShowIcon="true"
        Placeholder="Search state..."
        ButtonPlaceholder="Select State" />
</div>

<div class="form-group mt-3">
    <label>City:</label>
    <CityPicker 
        @bind-Value="ActiveCity"
        CountryCode="@ActiveCountry?.CountryCode"
        StateId="@ActiveState?.Id"
        Theme="PickerTheme.Forest"
        Roundness="PickerRoundness.Medium"
        ShowIcon="true"
        Placeholder="Search city..."
        ButtonPlaceholder="Select City" />
</div>

@code {
    private Country? ActiveCountry { get; set; }
    private State? ActiveState { get; set; }
    private City? ActiveCity { get; set; }
}
```

---

## ⚙️ Component API Reference

### `<CountryPicker>`

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Value` | `Country?` | `null` | Current selected country. Supports two-way binding via `@bind-Value`. |
| `ValueChanged` | `EventCallback<Country?>` | — | Callback triggered when the selected country changes. |
| `OnSelected` | `EventCallback<Country>` | — | Callback triggered upon selection (matches Flutter's `onSelect` callback). |
| `FavoriteCountries` | `IEnumerable<string>?` | `null` | Optional list of 2-letter ISO codes (e.g., `["US", "GB"]`) to pin in a priority "Favorites" section at the top. |
| `ExcludeCountries` | `IEnumerable<string>?` | `null` | Optional list of 2-letter ISO codes to completely exclude/hide from selection. |
| `CountryFilter` | `IEnumerable<string>?` | `null` | Optional list of 2-letter ISO codes. If provided, *only* these countries will be shown. |
| `ShowPhoneCode` | `bool` | `false` | When true, includes dial codes (e.g., `+1`, `+30`) in both the button label and popover items. |
| `ShowSearch` | `bool` | `true` | Toggle the search input box inside the dropdown. |
| `Theme` | `PickerTheme` | `PickerTheme.Light` | Visual styling theme (`Light`, `Dark`, `Blue`, `Forest`). |
| `Roundness` | `PickerRoundness` | `PickerRoundness.Medium` | Corner radius layout style (`None`, `Small`, `Medium`, `Large`, `Full`). |
| `Class` | `string` | `""` | Optional custom CSS class applied to the outer container. |
| `Style` | `string` | `""` | Optional inline CSS styles applied to the outer container. |
| `Placeholder` | `string` | `"Search country..."` | Placeholder text for the search input. |
| `ButtonPlaceholder` | `string` | `"Select country"` | Default text shown in the trigger button when nothing is selected. |
| `Disabled` | `bool` | `false` | Sets the component to an inactive, non-interactive state. |

---

### `<StatePicker>`

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Value` | `State?` | `null` | Selected state/province. Supports two-way binding via `@bind-Value`. |
| `ValueChanged` | `EventCallback<State?>` | — | Callback triggered when the selected state changes. |
| `OnSelected` | `EventCallback<State>` | — | Callback triggered upon selection. |
| `CountryCode` | `string?` | `null` | **(Required)** The 2-letter ISO country code to load states for. Changing this resets the `Value` to null. |
| `ShowIcon` | `bool` | `true` | Toggle whether the map icon (`🗺️`) displays in the button and dropdown list. |
| `ShowSearch` | `bool` | `true` | Toggle the search input box inside the dropdown. |
| `Theme` | `PickerTheme` | `PickerTheme.Light` | Visual styling theme (`Light`, `Dark`, `Blue`, `Forest`). |
| `Roundness` | `PickerRoundness` | `PickerRoundness.Medium` | Corner radius layout style (`None`, `Small`, `Medium`, `Large`, `Full`). |
| `Class` | `string` | `""` | Optional custom CSS class applied to the outer container. |
| `Style` | `string` | `""` | Optional inline CSS styles applied to the outer container. |
| `Placeholder` | `string` | `"Search state..."` | Placeholder text for the search input. |
| `ButtonPlaceholder` | `string` | `"Select state"` | Default trigger button label when nothing is selected. |
| `Disabled` | `bool` | `false` | Disables the component. If `CountryCode` is empty or null, the component is automatically disabled. |

---

### `<CityPicker>`

| Parameter | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Value` | `City?` | `null` | Selected city. Supports two-way binding via `@bind-Value`. |
| `ValueChanged` | `EventCallback<City?>` | — | Callback triggered when the selected city changes. |
| `OnSelected` | `EventCallback<City>` | — | Callback triggered upon selection. |
| `CountryCode` | `string?` | `null` | ISO 2-letter country code for validation/context. |
| `StateId` | `int?` | `null` | **(Required)** The database identifier of the state to load cities for. Changing this resets the `Value` to null. |
| `ShowIcon` | `bool` | `true` | Toggle whether the city icon (`🏙️`) displays in the button and dropdown list. |
| `ShowSearch` | `bool` | `true` | Toggle the search input box inside the dropdown. |
| `Theme` | `PickerTheme` | `PickerTheme.Light` | Visual styling theme (`Light`, `Dark`, `Blue`, `Forest`). |
| `Roundness` | `PickerRoundness` | `PickerRoundness.Medium` | Corner radius layout style (`None`, `Small`, `Medium`, `Large`, `Full`). |
| `Class` | `string` | `""` | Optional custom CSS class applied to the outer container. |
| `Style` | `string` | `""` | Optional inline CSS styles applied to the outer container. |
| `Placeholder` | `string` | `"Search city..."` | Placeholder text for the search input. |
| `ButtonPlaceholder` | `string` | `"Select city"` | Default trigger button label when nothing is selected. |
| `Disabled` | `bool` | `false` | Disables the component. If `StateId` is null, the component is automatically disabled. |

---

## 🛠️ Service APIs

If you need to query geographic data programmatically outside of the UI components, you can inject and use `CountryService` or `LocationService` directly in your code.

### `CountryService`

Provides access to the raw database of over 240+ countries.

- `GetAll()`: Returns all configured countries alphabetically sorted.
- `GetByCountryCode(string countryCode)`: Looks up a country by its 2-letter ISO code (case-insensitive).
- `GetByPhoneCode(string phoneCode)`: Finds all countries matching an international phone code.

### `LocationService`

Manages states and cities based on hierarchical relationships.

- `GetStates(string countryCode)`: Retrieves states/provinces for a given 2-letter country code.
- `GetCities(string countryCode, int stateId)`: Retrieves cities for a given country code and state ID.

---

## 🎨 Styles & Customization

The components use modern **Blazor CSS Isolation**, which means all styles are fully isolated to the `CountryPicker.Blazor` library and won't leak or conflict with your host application styles.

### Using CSS Variables

Our styling engine utilizes CSS Custom Properties (`--cp-*`) cascading down from the `.country-picker-container` root. If you want custom visual properties not covered by the enums, you can override variables in your global app stylesheet:

```css
/* Overriding CSS Custom Properties globally */
.country-picker-container {
    --cp-accent: #ff4081 !important; /* Change active focus/borders to hot pink */
    --cp-font: 'Consolas', monospace !important; /* Custom font family override */
    max-width: 100% !important; /* Allow full-width layouts */
}
```

Available CSS variables to override:
*   `--cp-bg`: Component background colors
*   `--cp-trigger-bg`: Select button background
*   `--cp-text`: Primary text color
*   `--cp-text-muted`: Placeholder/arrow icons
*   `--cp-border`: Form boundary borders
*   `--cp-accent`: Highlight / Focus boundaries
*   `--cp-dropdown-bg`: Popover container background
*   `--cp-radius`: Border radius corners

---

## 📄 License & Attribution

This library is licensed under the **MIT License**.
