using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Erasmove.Models;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IVoyageService _voyageService;
    private readonly IStateService _stateService;

    [ObservableProperty] public partial ObservableCollection<Voyage> Voyages { get; set; } = [];
    [ObservableProperty] public partial Voyage? SelectedVoyage { get; set; }

    public event Action<List<VoyageEtapeDetail>>? ItineraireChanged;

    public HomeViewModel(IVoyageService voyageService, IStateService stateService)
    {
        _voyageService = voyageService;
        _stateService = stateService;
    }

    public async Task LoadAsync()
    {
        var user = _stateService.CurrentUser;
        if (user == null) return;

        var list = await _voyageService.GetVoyagesByUserAsync(user.Id);
        Voyages = new ObservableCollection<Voyage>(list);
    }

    partial void OnSelectedVoyageChanged(Voyage? value) => _ = LoadItineraireAsync(value);

    private async Task LoadItineraireAsync(Voyage? voyage)
    {
        if (voyage == null)
        {
            ItineraireChanged?.Invoke([]);
            return;
        }

        var etapes = await _voyageService.GetItineraireVoyageAsync(voyage.Id);
        ItineraireChanged?.Invoke(etapes);
    }
}
