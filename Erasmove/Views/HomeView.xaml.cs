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

        HomeMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        HomeMap.Map.Navigator.OverrideZoomBounds = new MMinMax(100, 20000);
    }
}