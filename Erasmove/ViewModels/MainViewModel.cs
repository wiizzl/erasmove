using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;
using Erasmove.Models;
using Mapsui.UI.Maui;
using System.Globalization;

namespace Erasmove.ViewModels;

public class CountToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value > 0;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}

public class CountToHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Math.Min((int)value * 45, 150);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}

public enum TransportMode { Drive, Sea, Air, Train }

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

public class TimelineStep
{
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}

public partial class MainViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly HttpClient _httpClient;

    [ObservableProperty]
    private bool _isManager;

    [ObservableProperty]
    private bool _isNotManager;

    public ObservableCollection<string> StartCitySuggestions { get; } = new();
    public ObservableCollection<string> EndCitySuggestions { get; } = new();

    private string _startPoint = string.Empty;
    public string StartPoint
    {
        get => _startPoint;
        set
        {
            if (SetProperty(ref _startPoint, value))
            {
                DebounceSearchCity(value, StartCitySuggestions);
            }
        }
    }

    private string _destination = string.Empty;
    public string Destination
    {
        get => _destination;
        set
        {
            if (SetProperty(ref _destination, value))
            {
                DebounceSearchCity(value, EndCitySuggestions);
            }
        }
    }

    private CancellationTokenSource? _searchCts;

    private async void DebounceSearchCity(string query, ObservableCollection<string> listToUpdate)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            await Task.Delay(800, token); // Attendre 800ms que l'utilisateur arrête de taper

            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                MainThread.BeginInvokeOnMainThread(() => listToUpdate.Clear());
                return;
            }

            var cities = await SearchCityNameAsync(query);

            if (!token.IsCancellationRequested)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    listToUpdate.Clear();
                    foreach (var c in cities) listToUpdate.Add(c);
                });
            }
        }
        catch { }
    }

    private async Task<List<string>> SearchCityNameAsync(string query)
    {
        var cities = new List<string>();
        try
        {
            var url = $"https://nominatim.openstreetmap.org/search?city={Uri.EscapeDataString(query)}&format=json&limit=5";
            var response = await _httpClient.GetStringAsync(url);
            var results = JsonSerializer.Deserialize<List<NominatimCityResult>>(response);

            if (results != null)
            {
                foreach (var r in results.Where(x => !string.IsNullOrEmpty(x.name)))
                {
                    string label = r.name;
                    if (!string.IsNullOrEmpty(r.country)) label += $", {r.country}";
                    else if (!string.IsNullOrEmpty(r.display_name)) label = r.display_name.Split(',')[0];

                    if (!cities.Contains(label)) cities.Add(label);
                }
            }
        }
        catch { }
        return cities;
    }

    [RelayCommand]
    private void SelectStartCity(string city)
    {
        StartPoint = city;
        StartCitySuggestions.Clear();
    }

    [RelayCommand]
    private void SelectEndCity(string city)
    {
        Destination = city;
        EndCitySuggestions.Clear();
    }

    public ObservableCollection<WaypointModel> Waypoints { get; } = new();
    public ObservableCollection<TimelineStep> ItineraryTimeline { get; } = new();

    public event Action<List<Position>, List<RouteSegment>, double>? OnRouteCalculated;

    public MainViewModel(AuthService authService, Erasmove.Repositories.TravelerRepository travelerRepo, Erasmove.Repositories.TripRepository tripRepo)
    {
        _authService = authService;
        _travelerRepo = travelerRepo;
        _tripRepo = tripRepo;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ErasmoveMauiApp");
    }

    private readonly Erasmove.Repositories.TravelerRepository _travelerRepo;
    private readonly Erasmove.Repositories.TripRepository _tripRepo;

    public ObservableCollection<Traveler> Travelers { get; } = new();

    [ObservableProperty]
    private Traveler? _selectedTraveler;

    public ObservableCollection<Trip> AssignedTrips { get; } = new();

    private Trip? _selectedAssignedTrip;
    public Trip? SelectedAssignedTrip
    {
        get => _selectedAssignedTrip;
        set
        {
            if (SetProperty(ref _selectedAssignedTrip, value))
            {
                OnPropertyChanged(nameof(IsTripSelected));
            }
        }
    }

    public bool IsTripSelected => SelectedAssignedTrip != null;

    [ObservableProperty]
    private DateTime _departureDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan _departureTime = new TimeSpan(8, 0, 0);

    [ObservableProperty]
    private string _tripName = string.Empty;

    public async void InitializeData()
    {
        IsManager = _authService.IsManager;
        IsNotManager = !_authService.IsManager;

        try
        {
            Travelers.Clear();
            AssignedTrips.Clear();

            if (IsManager)
            {
                var travelers = await _travelerRepo.GetAllAsync();
                foreach (var t in travelers) Travelers.Add(t);
            }
            else if (_authService.CurrentUser != null)
            {
                var trips = await _tripRepo.GetTripsByTravelerIdAsync(_authService.CurrentUser.Id);
                foreach (var t in trips) AssignedTrips.Add(t);
            }
        }
        catch { }
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
    private async Task LoadTrip()
    {
        if (SelectedAssignedTrip == null)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Erreur", "Veuillez sélectionner un voyage.", "OK");
            return;
        }

        // Setup the fields so it can calculate the route map
        _startPoint = string.IsNullOrWhiteSpace(SelectedAssignedTrip.StartCity) ? "Paris" : SelectedAssignedTrip.StartCity;
        OnPropertyChanged(nameof(StartPoint));

        _destination = string.IsNullOrWhiteSpace(SelectedAssignedTrip.EndCity) ? "Londres" : SelectedAssignedTrip.EndCity;
        OnPropertyChanged(nameof(Destination));

        ItineraryTimeline.Clear();

        if (!string.IsNullOrWhiteSpace(SelectedAssignedTrip.WaypointsJson))
        {
            try
            {
                var steps = JsonSerializer.Deserialize<List<TimelineStep>>(SelectedAssignedTrip.WaypointsJson);
                if (steps != null)
                {
                    foreach(var s in steps) ItineraryTimeline.Add(s);
                }
            }
            catch { }
        }

        // Run the route calculation map rendering
        await SearchRoute();
    }

    [RelayCommand]
    private async Task SearchRoute()
    {
        if (string.IsNullOrWhiteSpace(_startPoint) || string.IsNullOrWhiteSpace(_destination))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Erreur", "Veuillez entrer un point de départ et une destination", "OK");
            return;
        }

        StartCitySuggestions.Clear();
        EndCitySuggestions.Clear();

        var stops = new List<(string Name, Position Pos)>();

        var startPos = await GeocodeAsync(_startPoint);
        if (startPos != null) stops.Add((_startPoint, startPos.Value));

        foreach (var wp in Waypoints)
        {
            if (!string.IsNullOrWhiteSpace(wp.Name))
            {
                var wpPos = await GeocodeAsync(wp.Name);
                if (wpPos != null) stops.Add((wp.Name, wpPos.Value));
            }
        }

        var endPos = await GeocodeAsync(_destination);
        if (endPos != null) stops.Add((_destination, endPos.Value));

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
                        segments.AddRange(driveToPort.Value.segments);
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
                        segments.AddRange(driveFromPort.Value.segments);
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
                segments.AddRange(drive.Value.segments);
                totalDistanceKm += drive.Value.dist;
            }
        }

        OnRouteCalculated?.Invoke(routePositions, segments, totalDistanceKm);

        if (IsManager)
        {
            if (SelectedTraveler == null)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Info", "L'itinéraire est calculé, mais vous devez sélectionner un utilisateur pour l'enregistrer.", "OK");
                return;
            }

            try
            {
                var timeline = new List<TimelineStep>();
                DateTime currentSegmentTime = DepartureDate.Add(DepartureTime);

                timeline.Add(new TimelineStep { Time = currentSegmentTime.ToString("HH:mm"), Icon = "📍", Description = $"Départ de {_startPoint}", Detail = DepartureDate.ToString("dd/MM/yyyy") });

                double durationHours = 0;
                foreach (var seg in segments)
                {
                    double segDist = GetDistanceOfSegment(seg.Geometry);
                    double segHours = 0;
                    string icon = "🚗";
                    string desc = "Trajet en voiture";

                    switch (seg.Mode)
                    {
                        case TransportMode.Drive: segHours = segDist / 80.0; icon = "🚗"; desc = "Conduite (Voiture/Bus)"; break;
                        case TransportMode.Train: segHours = segDist / 150.0 + 0.5; icon = "🚆"; desc = "Trajet en Train"; break;
                        case TransportMode.Air: segHours = segDist / 800.0 + 2.0; icon = "✈️"; desc = "Vol (Avion)"; break;
                        case TransportMode.Sea: segHours = segDist / 35.0 + 1.0; icon = "⛴️"; desc = "Traversée maritime"; break;
                    }

                    if (segDist > 5) // On ignore les micro-trajets de moins de 5km dans les logs textuels
                    {
                        currentSegmentTime = currentSegmentTime.AddHours(segHours);
                        timeline.Add(new TimelineStep { 
                            Time = currentSegmentTime.ToString("HH:mm"), 
                            Icon = icon, 
                            Description = desc, 
                            Detail = $"{(int)segDist} km - {TimeSpan.FromHours(segHours):hh\\:mm} environ" 
                        });
                    }
                    durationHours += segHours;
                }

                timeline.Add(new TimelineStep { Time = currentSegmentTime.ToString("HH:mm"), Icon = "🏁", Description = $"Arrivée à {_destination}", Detail = currentSegmentTime.ToString("dd/MM/yyyy") });

                // Sauvegarde du beau JSON dans la bdd
                string waypointsJson = JsonSerializer.Serialize(timeline);

                // Construction du nom du voyage par défaut
                string finalTripName = string.IsNullOrWhiteSpace(TripName) 
                    ? $"{_startPoint} ➔ {_destination} ({Waypoints.Count} escale{(Waypoints.Count > 1 ? "s" : "")})" 
                    : TripName;

                var trip = new Trip
                {
                    TravelerId = SelectedTraveler.Id,
                    DepartureDate = DepartureDate.Add(DepartureTime),
                    ArrivalDate = DepartureDate.Add(DepartureTime).AddHours(durationHours),
                    StartCity = _startPoint,
                    EndCity = _destination,
                    DurationText = TimeSpan.FromHours(durationHours).ToString(@"hh\:mm"),
                    TripName = finalTripName,
                    WaypointsJson = waypointsJson,
                    PlaceId = 1,
                    TransportId = 1
                };

                await _tripRepo.AddAsync(trip);

                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Succès", "Voyage enregistré et assigné.\nHeure d'arrivée estimée : " + trip.ArrivalDate.ToString("HH:mm"), "OK");
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Erreur BD", "L'itinéraire est affiché, mais la sauvegarde a échoué (les colonnes en base de données doivent être mises à jour).", "OK");
            }
        }
    }

    private double GetDistanceOfSegment(List<Position> geom)
    {
        double dist = 0;
        for (int i = 0; i < geom.Count - 1; i++)
        {
            dist += GetHaversineDistance(geom[i], geom[i+1]);
        }
        return dist;
    }

    private async Task<(List<RouteSegment> segments, double dist)?> GetDrivingRouteAsync(Position a, Position b)
    {
        try
        {
            var coordsStr = $"{a.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{a.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)};{b.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{b.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            var url = $"https://router.project-osrm.org/route/v1/driving/{coordsStr}?overview=full&geometries=geojson&steps=true";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<OsrmResponse>(response);

            if (data?.routes != null && data.routes.Count > 0)
            {
                var route = data.routes[0];
                var dist = (route.distance ?? 0) / 1000.0;
                var segments = new List<RouteSegment>();

                if (route.legs != null)
                {
                    foreach (var leg in route.legs)
                    {
                        if (leg.steps != null)
                        {
                            foreach (var step in leg.steps)
                            {
                                if (step.geometry?.coordinates != null)
                                {
                                    var geom = new List<Position>();
                                    foreach (var c in step.geometry.coordinates)
                                    {
                                        if (c.Count >= 2) geom.Add(new Position(c[1], c[0]));
                                    }

                                    TransportMode mode = TransportMode.Drive;
                                    if (step.mode == "ferry") 
                                        mode = TransportMode.Sea;
                                    else if (dist > 400 && step.mode != "ferry") 
                                        mode = TransportMode.Train;

                                    if (segments.Count > 0 && segments.Last().Mode == mode)
                                    {
                                        segments.Last().Geometry.AddRange(geom);
                                    }
                                    else
                                    {
                                        segments.Add(new RouteSegment { Mode = mode, Geometry = geom });
                                    }
                                }
                            }
                        }
                    }
                }

                if (segments.Count == 0 && route.geometry?.coordinates != null)
                {
                    var geom = new List<Position>();
                    foreach (var c in route.geometry.coordinates)
                    {
                        if (c.Count >= 2) geom.Add(new Position(c[1], c[0]));
                    }
                    var mode = dist > 400 ? TransportMode.Train : TransportMode.Drive;
                    segments.Add(new RouteSegment { Mode = mode, Geometry = geom });
                }

                return (segments, dist);
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

    private class NominatimCityResult
    {
        public string name { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string display_name { get; set; } = string.Empty;
    }

    private class OsrmResponse
    {
        public List<OsrmRoute>? routes { get; set; }
    }

    private class OsrmRoute
    {
        public OsrmGeometry? geometry { get; set; }
        public double? distance { get; set; }
        public List<OsrmLeg>? legs { get; set; }
    }

    private class OsrmLeg
    {
        public List<OsrmStep>? steps { get; set; }
    }

    private class OsrmStep
    {
        public string? mode { get; set; }
        public OsrmGeometry? geometry { get; set; }
    }

    private class OsrmGeometry
    {
        public List<List<double>>? coordinates { get; set; }
    }
}