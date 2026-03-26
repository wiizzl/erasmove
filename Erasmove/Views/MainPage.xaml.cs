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

    private void DrawRoute(List<Position> routePoints)
    {
        MapView.Pins.Clear();
        MapView.Drawables.Clear();

        if (routePoints == null || routePoints.Count == 0) return;

        // Start pin
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = routePoints[0],
            Label = "Départ",
            Color = Colors.Green
        });

        // Destination pin
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = routePoints[^1],
            Label = "Arrivée",
            Color = Colors.Red
        });

        // Waypoint pins
        for (int i = 1; i < routePoints.Count - 1; i++)
        {
            MapView.Pins.Add(new Pin(MapView)
            {
                Position = routePoints[i],
                Label = $"Escale {i}",
                Color = Colors.Orange
            });
        }

        // Draw line
        var route = new Polyline
        {
            StrokeColor = new Color(0.2f, 0.5f, 1.0f),
            StrokeWidth = 4f
        };

        foreach (var p in routePoints)
        {
            route.Positions.Add(p);
        }

        MapView.Drawables.Add(route);

        // Center map
        if (routePoints.Count > 1)
        {
            var centerLat = (routePoints[0].Latitude + routePoints[^1].Latitude) / 2;
            var centerLon = (routePoints[0].Longitude + routePoints[^1].Longitude) / 2;
            var center = Mapsui.Projections.SphericalMercator.FromLonLat(centerLon, centerLat);

            MapView.Map.Navigator.CenterOnAndZoomTo(
                new MPoint(center.x, center.y),
                MapView.Map.Navigator.Resolutions[6]);

            // Rendre le panneau d'info visible
            RouteInfoFrame.IsVisible = true;
        }
    }
}
