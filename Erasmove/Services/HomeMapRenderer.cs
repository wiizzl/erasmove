using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using System.Linq;

namespace Erasmove.Services;

public class HomeMapRenderer : IHomeMapRenderer
{
    public void Render(MapControl mapControl, IEnumerable<VoyageEtapeDetail> etapes)
    {
        if (mapControl.Map is null)
        {
            mapControl.Map = new Mapsui.Map();
        }

        var layersToRemove = mapControl.Map.Layers.Where(layer => layer.Name is "Routes" or "Pins").ToList();
        foreach (var layer in layersToRemove)
        {
            mapControl.Map.Layers.Remove(layer);
        }

        var itinerary = etapes.ToList();
        if (itinerary.Count == 0)
        {
            mapControl.Refresh();
            return;
        }

        var waypoints = new List<(double X, double Y, string Label)>();
        foreach (var etape in itinerary)
        {
            var (dx, dy) = SphericalMercator.FromLonLat(etape.LonDepart, etape.LatDepart);
            waypoints.Add((dx, dy, etape.NomDepart));

            var (ax, ay) = SphericalMercator.FromLonLat(etape.LonArrivee, etape.LatArrivee);
            waypoints.Add((ax, ay, etape.NomArrivee));
        }

        var coords = waypoints.Select(w => new Coordinate(w.X, w.Y)).ToArray();
        mapControl.Map.Layers.Add(new MemoryLayer
        {
            Name = "Routes",
            Features = [new GeometryFeature { Geometry = new LineString(coords) }],
            Style = new Mapsui.Styles.VectorStyle
            {
                Line = new Mapsui.Styles.Pen(new Mapsui.Styles.Color(30, 136, 229, 255), 3)
            }
        });

        mapControl.Map.Layers.Add(new MemoryLayer
        {
            Name = "Pins",
            Features = waypoints
                .Select(w => (IFeature)new PointFeature(new MPoint(w.X, w.Y)))
                .ToList(),
            Style = new Mapsui.Styles.SymbolStyle
            {
                SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(220, 53, 69, 255)),
                Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 1.5f),
                SymbolScale = 0.4
            }
        });

        var minX = waypoints.Min(w => w.X);
        var maxX = waypoints.Max(w => w.X);
        var minY = waypoints.Min(w => w.Y);
        var maxY = waypoints.Max(w => w.Y);
        var padding = Math.Max(maxX - minX, maxY - minY) * 0.3;
        mapControl.Map.Navigator.ZoomToBox(new MRect(minX - padding, minY - padding, maxX + padding, maxY + padding));
        mapControl.Refresh();
    }
}
