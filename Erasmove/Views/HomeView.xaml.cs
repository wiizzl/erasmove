using Mapsui;
using Mapsui.Tiling;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;
using System.ComponentModel;

namespace Erasmove.Views;

public partial class HomeView : ContentPage
{
    private readonly HomeViewModel _viewModel;
    private readonly IHomeMapRenderer _mapRenderer;

    public HomeView(HomeViewModel viewModel, IHomeMapRenderer mapRenderer)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _mapRenderer = mapRenderer;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        HomeMap.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        HomeMap.Map.Navigator.OverrideZoomBounds = new MMinMax(100, 20000);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);
        _mapRenderer.Render(HomeMap, _viewModel.SelectedVoyageItinerary);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HomeViewModel.SelectedVoyageItinerary))
        {
            _mapRenderer.Render(HomeMap, _viewModel.SelectedVoyageItinerary);
        }
    }
}