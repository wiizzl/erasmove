using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Ajout de la couche de tuiles OpenStreetMap (fond de carte)
        MapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Trajet fixe Paris → Marseille affiché au lancement
        AddParisMarseille();
    }

    /// <summary>
    /// Affiche un itinéraire Paris → Marseille en dur sur la carte.
    ///
    /// COMMENT ÇA MARCHE :
    /// 1. On crée des Position(lat, lon) — coordonnées GPS classiques.
    /// 2. On crée un Pin pour chaque point via new Pin(MapView) puis on l'ajoute à MapView.Pins.
    ///    - Le Pin est l'équivalent d'un marqueur sur la carte.
    ///    - La propriété Color détermine la couleur de l'icône.
    ///    - La propriété Label affiche un texte au clic sur le pin.
    /// 3. On crée un Polyline (ligne reliant des points) et on y ajoute les positions.
    ///    - StrokeColor = couleur du trait, StrokeWidth = épaisseur en pixels.
    ///    - On l'ajoute à MapView.Drawables (collection de formes dessinées sur la carte).
    ///    - ATTENTION : c'est une ligne droite entre les points, pas un vrai itinéraire routier.
    /// 4. On centre la carte entre les deux villes avec CenterOnAndZoomTo.
    ///    - SphericalMercator.FromLonLat convertit lon/lat → coordonnées Mercator (système interne de Mapsui).
    ///    - Resolutions[6] est un niveau de zoom prédéfini (~échelle France entière).
    /// </summary>
    private void AddParisMarseille()
    {
        var paris = new Position(48.8566, 2.3522);
        var marseille = new Position(43.2965, 5.3698);

        // Pin de départ (vert)
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = paris,
            Label = "Paris",
            Color = Colors.Green
        });

        // Pin d'arrivée (rouge)
        MapView.Pins.Add(new Pin(MapView)
        {
            Position = marseille,
            Label = "Marseille",
            Color = Colors.Red
        });

        // Polyline reliant les deux villes (trait bleu)
        var route = new Polyline
        {
            StrokeColor = new Color(0.2f, 0.5f, 1.0f),
            StrokeWidth = 4f
        };
        route.Positions.Add(paris);
        route.Positions.Add(marseille);
        MapView.Drawables.Add(route);

        // Centrer la carte entre Paris et Marseille
        var centerLat = (paris.Latitude + marseille.Latitude) / 2;
        var centerLon = (paris.Longitude + marseille.Longitude) / 2;
        var center = Mapsui.Projections.SphericalMercator.FromLonLat(centerLon, centerLat);

        MapView.Map.Navigator.CenterOnAndZoomTo(
            new Mapsui.MPoint(center.x, center.y),
            MapView.Map.Navigator.Resolutions[6]);
    }
}
