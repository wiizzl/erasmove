using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;
using Mapsui.UI.Maui;

namespace Erasmove.ViewModels;

public enum TransportMode { Drive, Sea, Air }

public class RouteSegment
{
    public TransportMode Mode { get; set; }
    public List<Position> Geometry { get; set; } = new();
}

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

    public event Action<List<Position>, List<RouteSegment>, double>? OnRouteCalculated;

    public MainViewModel(AuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ErasmoveMauiApp");
    }

    private static readonly List<(string Name, Position Pos)> MajorAirports = new()
    {
        ("Aéroport de Paris-CDG", new Position(49.0097, 2.5479)),
        ("Aéroport de Londres-Heathrow", new Position(51.4700, -0.4543)),
        ("Aéroport JFK - New York", new Position(40.6413, -73.7781)),
        ("Aéroport de Los Angeles", new Position(33.9416, -118.4085)),
        ("Aéroport de Tokyo-Haneda", new Position(35.5494, 139.7798)),
        ("Aéroport de Dubaï", new Position(25.2532, 55.3657)),
        ("Aéroport de Sydney", new Position(-33.9399, 151.1753)),
        ("Aéroport de São Paulo-Guarulhos", new Position(-23.4306, -46.4731)),
        ("Aéroport de Singapour-Changi", new Position(1.3644, 103.9915)),
        ("Aéroport de Francfort", new Position(50.0333, 8.5706)),
        ("Aéroport de Madrid-Barajas", new Position(40.4839, -3.5680)),
        ("Aéroport de Rome Fiumicino", new Position(40.7997, 12.2462)),
        ("Aéroport de Toronto-Pearson", new Position(43.6777, -79.6248))
    };

    private static readonly List<(string Name, Position Pos)> MajorPorts = new()
    {
        ("Port du Havre", new Position(49.4833, 0.1000)),
        ("Port de New York", new Position(40.6600, -74.0400)),
        ("Port de Shanghai", new Position(31.2304, 121.4737)),
        ("Port de Marseille", new Position(43.3100, 5.3500)),
        ("Port de Rotterdam", new Position(51.9000, 4.2800)),
        ("Port de Los Angeles", new Position(33.7200, -118.2600)),
        ("Port de Sydney", new Position(-33.8500, 151.2000)),
        ("Port de Bastia", new Position(42.7028, 9.4500)),
        ("Port d'Ajaccio", new Position(41.9267, 8.7369)),
        ("Port de Toulon", new Position(43.1242, 5.9280))
    };

    private (string Name, Position Pos) GetNearestHub(Position cityPos, TransportMode mode)
    {
        var hubs = mode == TransportMode.Air ? MajorAirports : MajorPorts;
        return hubs.OrderBy(h => GetHaversineDistance(cityPos, h.Pos)).First();
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

        var stops = new List<(string Name, Position Pos)>();

        var startPos = await GeocodeAsync(StartPoint);
        if (startPos != null) stops.Add((StartPoint, startPos.Value));

        foreach (var wp in Waypoints)
        {
            if (!string.IsNullOrWhiteSpace(wp.Name))
            {
                var wpPos = await GeocodeAsync(wp.Name);
                if (wpPos != null) stops.Add((wp.Name, wpPos.Value));
            }
        }

        var endPos = await GeocodeAsync(Destination);
        if (endPos != null) stops.Add((Destination, endPos.Value));

        if (stops.Count < 2)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Erreur", "Impossible de trouver les coordonnées de ces villes.", "OK");
            return;
        }

        var segments = new List<RouteSegment>();
        double totalDistanceKm = 0;
        var routePositions = stops.Select(s => s.Pos).ToList();

        for (int i = 0; i < stops.Count - 1; i++)
        {
            var a = stops[i];
            var b = stops[i + 1];

            var drive = await GetDrivingRouteAsync(a.Pos, b.Pos);
            var directDist = GetHaversineDistance(a.Pos, b.Pos);

            // Multimodal if OSRM fails (ocean crossing) or distance is very large (> 1500km = plane)
            if (drive == null || (directDist > 1500))
            {
                // Un trajet terrestre échoué, on essaie ferry s'ils sont relativement proches (<800km) ou si spécifiquement demandé.
                // Mais par défaut, un trajet impossible ou > 1500km passe en multimodal.
                TransportMode mode = (directDist > 1500) ? TransportMode.Air : TransportMode.Sea;

                var portA = GetNearestHub(a.Pos, mode);
                var portB = GetNearestHub(b.Pos, mode);

                // 1. Drive to local port/airport
                if (GetHaversineDistance(a.Pos, portA.Pos) > 2) // > 2km
                {
                    var driveToPort = await GetDrivingRouteAsync(a.Pos, portA.Pos);
                    if (driveToPort != null)
                    {
                        segments.Add(new RouteSegment { Mode = TransportMode.Drive, Geometry = driveToPort.Value.geom });
                        totalDistanceKm += driveToPort.Value.dist;
                    }
                    else
                    {
                        segments.Add(new RouteSegment { Mode = TransportMode.Drive, Geometry = new List<Position> { a.Pos, portA.Pos } });
                        totalDistanceKm += GetHaversineDistance(a.Pos, portA.Pos);
                    }
                }

                // 2. Sail or Fly across
                segments.Add(new RouteSegment { Mode = mode, Geometry = new List<Position> { portA.Pos, portB.Pos } });
                totalDistanceKm += GetHaversineDistance(portA.Pos, portB.Pos);

                // 3. Drive from remote port/airport to destination
                if (GetHaversineDistance(portB.Pos, b.Pos) > 2)
                {
                    var driveFromPort = await GetDrivingRouteAsync(portB.Pos, b.Pos);
                    if (driveFromPort != null)
                    {
                        segments.Add(new RouteSegment { Mode = TransportMode.Drive, Geometry = driveFromPort.Value.geom });
                        totalDistanceKm += driveFromPort.Value.dist;
                    }
                    else
                    {
                        segments.Add(new RouteSegment { Mode = TransportMode.Drive, Geometry = new List<Position> { portB.Pos, b.Pos } });
                        totalDistanceKm += GetHaversineDistance(portB.Pos, b.Pos);
                    }
                }
            }
            else
            {
                segments.Add(new RouteSegment { Mode = TransportMode.Drive, Geometry = drive.Value.geom });
                totalDistanceKm += drive.Value.dist;
            }
        }

        OnRouteCalculated?.Invoke(routePositions, segments, totalDistanceKm);
    }

    private async Task<(List<Position> geom, double dist)?> GetDrivingRouteAsync(Position a, Position b)
    {
        try
        {
            var coordsStr = $"{a.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{a.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)};{b.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{b.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            var url = $"https://router.project-osrm.org/route/v1/driving/{coordsStr}?overview=full&geometries=geojson";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<OsrmResponse>(response);

            if (data?.routes != null && data.routes.Count > 0)
            {
                var dist = (data.routes[0].distance ?? 0) / 1000.0;
                var geom = new List<Position>();
                if (data.routes[0].geometry?.coordinates != null)
                {
                    foreach (var c in data.routes[0].geometry.coordinates)
                    {
                        if (c.Count >= 2) geom.Add(new Position(c[1], c[0]));
                    }
                }
                return (geom, dist);
            }
        }
        catch { }
        return null;
    }

    private double GetHaversineDistance(Position a, Position b)
    {
        var dLat = (b.Latitude - a.Latitude) * Math.PI / 180.0;
        var dLon = (b.Longitude - a.Longitude) * Math.PI / 180.0;
        var aLat = a.Latitude * Math.PI / 180.0;
        var bLat = b.Latitude * Math.PI / 180.0;
        var val = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Sin(dLon/2)*Math.Sin(dLon/2) * Math.Cos(aLat) * Math.Cos(bLat);
        return 6371 * 2 * Math.Atan2(Math.Sqrt(val), Math.Sqrt(1-val));
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
            // Ignorer les erreurs
        }
        return null;
    }

    private class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
    }

    private class OsrmResponse
    {
        public List<OsrmRoute>? routes { get; set; }
    }

    private class OsrmRoute
    {
        public OsrmGeometry? geometry { get; set; }
        public double? distance { get; set; }
    }

    private class OsrmGeometry
    {
        public List<List<double>>? coordinates { get; set; }
    }
}