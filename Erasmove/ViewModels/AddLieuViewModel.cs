using CommunityToolkit.Mvvm.ComponentModel;
using Erasmove.Models;
using Erasmove.Models.Interfaces;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

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

    protected override async Task ExecuteUpdateAsync()
    {
        if (EditingItem is not Lieu lieu) return;

        lieu.Nom = Nom;
        lieu.Ville = Ville;
        lieu.Pays = Pays;
        lieu.Latitude = Latitude;
        lieu.Longitude = Longitude;

        await _lieuService.UpdateLieuAsync(lieu);
    }

    protected override void LoadItemData(IEntity item)
    {
        if (item is not Lieu lieu)
        {
            return;
        }

        Nom = lieu.Nom;
        Ville = lieu.Ville;
        Pays = lieu.Pays;
        Latitude = lieu.Latitude;
        Longitude = lieu.Longitude;
    }
}