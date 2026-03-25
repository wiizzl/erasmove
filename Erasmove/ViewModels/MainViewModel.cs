using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;
using Mapsui.UI.Maui;

namespace Erasmove.ViewModels;

public class WaypointModel : ObservableObject
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}

public partial class MainViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly HttpClient _httpClient;

    public bool IsManager => _authService.IsManager;
    public bool IsNotManager => !IsManager;

    [ObservableProperty]
    private string _startPoint = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    public ObservableCollection<WaypointModel> Waypoints { get; } = new();

    public event Action<List<Position>>? OnRouteCalculated;

    public MainViewModel(AuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ErasmoveMauiApp");
    }

    [RelayCommand]
    private void AddWaypoint()
    {
        if (Waypoints.Count < 5)
            Waypoints.Add(new WaypointModel());
    }

    [RelayCommand]
    private void RemoveWaypoint(WaypointModel waypoint)
    {
        if (waypoint != null && Waypoints.Contains(waypoint))
        {
            Waypoints.Remove(waypoint);
        }
    }

    [RelayCommand]
    private async Task SearchRoute()
    {
        if (string.IsNullOrWhiteSpace(StartPoint) || string.IsNullOrWhiteSpace(Destination))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Erreur", "Veuillez entrer un point de départ et une destination", "OK");
            return;
        }

        var routePositions = new List<Position>();

        var startPos = await GeocodeAsync(StartPoint);
        if (startPos != null) routePositions.Add(startPos.Value);

        foreach (var wp in Waypoints)
        {
            if (!string.IsNullOrWhiteSpace(wp.Name))
            {
                var wpPos = await GeocodeAsync(wp.Name);
                if (wpPos != null) routePositions.Add(wpPos.Value);
            }
        }

        var endPos = await GeocodeAsync(Destination);
        if (endPos != null) routePositions.Add(endPos.Value);

        if (routePositions.Count >= 2)
        {
            OnRouteCalculated?.Invoke(routePositions);
        }
        else
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Erreur", "Impossible de trouver les coordonnées de ces villes.", "OK");
        }
    }

    private async Task<Position?> GeocodeAsync(string cityName)
    {
        try
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json&limit=1";
            var response = await _httpClient.GetStringAsync(url);

            var results = JsonSerializer.Deserialize<List<NominatimResult>>(response);
            if (results != null && results.Count > 0)
            {
                if (double.TryParse(results[0].lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(results[0].lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                {
                    return new Position(lat, lon);
                }
            }
        }
        catch (Exception)
        {
            // Ignorer les erreurs de géocodage
        }
        return null;
    }

    private class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
    }
}