using Erasmove.ViewModels;
using Mapsui.Tiling;

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