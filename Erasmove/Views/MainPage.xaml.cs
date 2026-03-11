using Mapsui.Tiling;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}