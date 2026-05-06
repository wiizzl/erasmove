using Erasmove.Models;
using Erasmove.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;

namespace Erasmove.Views;

public partial class HomeView : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomeView(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        HomeMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        HomeMap.Map.Navigator.OverrideZoomBounds = new MMinMax(200, 20000);

        viewModel.ItineraireChanged += etapes =>
            MainThread.BeginInvokeOnMainThread(() => RenderItineraire(etapes));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private void RenderItineraire(List<VoyageEtapeDetail> etapes)
    {
        var toRemove = HomeMap.Map.Layers.Where(l => l.Name is "Routes" or "Pins").ToList();
        foreach (var layer in toRemove)
            HomeMap.Map.Layers.Remove(layer);

        if (etapes.Count == 0)
        {
            HomeMap.Refresh();
            return;
        }

        // Waypoints ordonnés : départ + arrivée de chaque étape
        var waypoints = new List<(double x, double y, string name)>();
        foreach (var etape in etapes)
        {
            var (dx, dy) = SphericalMercator.FromLonLat(etape.LonDepart, etape.LatDepart);
            waypoints.Add((dx, dy, etape.NomDepart));
            var (ax, ay) = SphericalMercator.FromLonLat(etape.LonArrivee, etape.LatArrivee);
            waypoints.Add((ax, ay, etape.NomArrivee));
        }

        // Ligne bleue reliant les étapes dans l'ordre
        var coords = waypoints.Select(w => new Coordinate(w.x, w.y)).ToArray();
        HomeMap.Map.Layers.Add(new MemoryLayer
        {
            Name = "Routes",
            Features = [new GeometryFeature { Geometry = new LineString(coords) }],
            Style = new Mapsui.Styles.VectorStyle
            {
                Line = new Mapsui.Styles.Pen(new Mapsui.Styles.Color(30, 136, 229), 3)
            }
        });

        // Points rouges sur chaque lieu
        var pins = waypoints
            .Select(w => (IFeature)new PointFeature(new MPoint(w.x, w.y)))
            .ToList();
        HomeMap.Map.Layers.Add(new MemoryLayer
        {
            Name = "Pins",
            Features = pins,
            Style = new Mapsui.Styles.SymbolStyle
            {
                SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(229, 57, 53)),
                Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 1.5f),
                SymbolScale = 0.4
            }
        });

        // Zoom pour englober tous les points avec marge
        var minX = waypoints.Min(w => w.x);
        var maxX = waypoints.Max(w => w.x);
        var minY = waypoints.Min(w => w.y);
        var maxY = waypoints.Max(w => w.y);
        var padding = Math.Max(100_000d, Math.Max(maxX - minX, maxY - minY) * 0.3);
        HomeMap.Map.Navigator.ZoomToBox(new MRect(minX - padding, minY - padding, maxX + padding, maxY + padding));

        HomeMap.Refresh();
    }
}
