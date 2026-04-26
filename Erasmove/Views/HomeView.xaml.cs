using Erasmove.ViewModels;
using Mapsui;
using Mapsui.Tiling;

namespace Erasmove.Views;

public partial class HomeView : ContentPage
{
    public HomeView(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        UserMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        UserMap.Map.Navigator.OverrideZoomBounds = new MMinMax(200, 20000);
    }
}