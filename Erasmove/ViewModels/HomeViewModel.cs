using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class HomeViewModel : BaseCatalogViewModel<Voyage>
{
    private readonly CurrentUserService _currentUserService;
    private readonly IVoyageService _voyageService;

    [ObservableProperty] public partial Voyage? SelectedVoyage { get; set; }
    [ObservableProperty] public partial ObservableCollection<VoyageEtapeDetail> SelectedVoyageItinerary { get; set; } = [];

    public HomeViewModel(CurrentUserService currentUserService, IVoyageService voyageService, INavigationService navigationService) : base(voyageService, navigationService, "AddVoyage")
    {
        _currentUserService = currentUserService;
        _voyageService = voyageService;
    }

    partial void OnSelectedVoyageChanged(Voyage? value)
    {
        _ = LoadSelectedVoyageItineraryAsync(value);
    }

    public override async Task LoadItemsAsync()
    {
        try
        {
            IsRefreshing = true;

            if (_currentUserService.CurrentUser == null)
            {
                Items.Clear();
                SelectedVoyage = null;
                SelectedVoyageItinerary = [];
                return;
            }

            var userVoyages = await _voyageService.GetVoyagesByUserAsync(_currentUserService.CurrentUser.Id);

            Items.Clear();
            foreach (var voyage in userVoyages)
            {
                Items.Add(voyage);
            }

            if (Items.Count > 0)
            {
                SelectedVoyage = Items[0];
            }
            else
            {
                SelectedVoyage = null;
                SelectedVoyageItinerary = [];
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    public async Task LoadSelectedVoyageItineraryAsync(Voyage? voyage)
    {
        if (voyage == null)
        {
            SelectedVoyageItinerary = [];
            return;
        }

        try
        {
            var itinerary = await _voyageService.GetItineraireVoyageAsync(voyage.Id);
            SelectedVoyageItinerary = new ObservableCollection<VoyageEtapeDetail>(itinerary);
        }
        catch
        {
            SelectedVoyageItinerary = [];
        }
    }
}