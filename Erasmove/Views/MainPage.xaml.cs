using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Erasmove.ViewModels;
using Mapsui;

namespace Erasmove.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // Add OpenStreetMap tile layer
        MapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Setup Map for routing display
        _viewModel.OnRouteCalculated += DrawRoute;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeData();
    }

    private void DrawRoute(List<Position> stops, List<RouteSegment> segments, double distanceKm)
    {
        MapView.Pins.Clear();
        MapView.Drawables.Clear();

        if (stops == null || stops.Count == 0) return;

        // Start pin
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = stops[0],
            Label = "Départ",
            Color = Colors.Green
        });

        // Destination pin
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = stops[^1],
            Label = "Arrivée",
            Color = Colors.Red
        });

        // Waypoint pins
        for (int i = 1; i < stops.Count - 1; i++)
        {
            MapView.Pins.Add(new Pin(MapView)
            {
                Position = stops[i],
                Label = $"Escale {i}",
                Color = Colors.Orange
            });
        }

        // Draw segments
        foreach (var seg in segments)
        {
            Color strokeColor = Colors.Gray; // Drive par défaut

            switch (seg.Mode)
            {
                case TransportMode.Drive: strokeColor = Colors.DarkGray; break; // Voiture = Gris foncé
                case TransportMode.Sea: strokeColor = Colors.DeepSkyBlue; break; // Bateau = Bleu ciel
                case TransportMode.Air: strokeColor = Colors.Magenta; break; // Avion = Magenta / Rose
                case TransportMode.Train: strokeColor = Colors.Orange; break; // Train = Orange
            }

            var route = new Polyline
            {
                StrokeColor = strokeColor,
                StrokeWidth = 5f
            };

            foreach (var p in seg.Geometry)
            {
                route.Positions.Add(p);
            }

            MapView.Drawables.Add(route);
        }

        // Center map
        if (stops.Count > 1)
        {
            // Calculate robust bounding box over all segments
            double minLat = 90, maxLat = -90, minLon = 180, maxLon = -180;

            foreach (var seg in segments)
            {
                foreach(var p in seg.Geometry)
                {
                    if(p.Latitude < minLat) minLat = p.Latitude;
                    if(p.Latitude > maxLat) maxLat = p.Latitude;
                    if(p.Longitude < minLon) minLon = p.Longitude;
                    if(p.Longitude > maxLon) maxLon = p.Longitude;
                }
            }

            // Fallback if nothing was added
            if (minLat == 90) { minLat = stops[0].Latitude; maxLat = stops[^1].Latitude; }
            if (minLon == 180) { minLon = stops[0].Longitude; maxLon = stops[^1].Longitude; }

            var centerLat = (minLat + maxLat) / 2;
            var centerLon = (minLon + maxLon) / 2;
            var center = Mapsui.Projections.SphericalMercator.FromLonLat(centerLon, centerLat);

            MapView.Map.Navigator.CenterOnAndZoomTo(
                new MPoint(center.x, center.y),
                MapView.Map.Navigator.Resolutions[5]);

            // Rendre le panneau d'info visible et afficher la distance
            RouteInfoFrame.IsVisible = true;
            DistanceLabel.Text = distanceKm > 0 ? $"Distance totale: {distanceKm:F0} km" : "Itinéraire approximatif";
        }
    }
}
