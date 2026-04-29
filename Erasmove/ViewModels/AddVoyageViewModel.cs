using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class AddVoyageViewModel : BaseAddViewModel
{
    private readonly IVoyageService _voyageService;
    private readonly ITrajetService _trajetService;
    private readonly ILieuService _lieuService;
    private readonly IUtilisateurService _utilisateurService;

    [ObservableProperty] public partial Lieu? SelectedDepart { get; set; }
    [ObservableProperty] public partial Lieu? SelectedArrivee { get; set; }
    [ObservableProperty] public partial Utilisateur? SelectedUtilisateur { get; set; }
    [ObservableProperty] public partial bool HasResult { get; set; }

    public ObservableCollection<Lieu> Villes { get; } = [];
    public ObservableCollection<Utilisateur> Utilisateurs { get; } = [];
    public ObservableCollection<Trajet> ItineraireCalcule { get; } = [];

    public AddVoyageViewModel(IVoyageService voyageService, ITrajetService trajetService, ILieuService lieuService, IUtilisateurService utilisateurService, INavigationService navigationService) : base(navigationService)
    {
        _voyageService = voyageService;
        _trajetService = trajetService;
        _lieuService = lieuService;
        _utilisateurService = utilisateurService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var lieux = await _lieuService.GetAllAsync();
        var utilisateurs = await _utilisateurService.GetAllAsync();

        Villes.Clear();
        foreach (var l in lieux)
        {
            Villes.Add(l);
        }

        Utilisateurs.Clear();
        foreach (var u in utilisateurs)
        {
            Utilisateurs.Add(u);
        }
    }

    protected override bool ValidateForm()
    {
        return SelectedUtilisateur != null && HasResult && ItineraireCalcule.Count > 0;
    }

    protected override async Task ExecuteSaveAsync()
    {

    }
}