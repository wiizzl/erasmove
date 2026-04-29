using CommunityToolkit.Mvvm.ComponentModel;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class AddLieuViewModel : BaseAddViewModel
{
    private readonly ILieuService _lieuService;

    [ObservableProperty] public partial string Nom { get; set; } = string.Empty;
    [ObservableProperty] public partial string Ville { get; set; } = string.Empty;
    [ObservableProperty] public partial string Pays { get; set; } = string.Empty;
    [ObservableProperty] public partial double Latitude { get; set; }
    [ObservableProperty] public partial double Longitude { get; set; }

    public AddLieuViewModel(ILieuService lieuService, INavigationService navigationService) : base(navigationService)
    {
        _lieuService = lieuService;
    }

    protected override bool ValidateForm()
    {
        return !string.IsNullOrWhiteSpace(Nom) &&
               !string.IsNullOrWhiteSpace(Ville) &&
               !string.IsNullOrWhiteSpace(Pays);
    }

    protected override async Task ExecuteSaveAsync()
    {
        var lieu = new Lieu
        {
            Nom = Nom,
            Ville = Ville,
            Pays = Pays,
            Latitude = Latitude,
            Longitude = Longitude
        };

        await _lieuService.AddLieuAsync(lieu);
    }
}