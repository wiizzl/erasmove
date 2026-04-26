using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class AddVoyageViewModel : BaseAddViewModel
{
    private readonly VoyageService _voyageService;
    private readonly TrajetService _trajetService;
    private readonly LieuService _lieuService;
    private readonly UtilisateurService _utilisateurService;

    [ObservableProperty] public partial Lieu? SelectedDepart { get; set; }
    [ObservableProperty] public partial Lieu? SelectedArrivee { get; set; }
    [ObservableProperty] public partial Utilisateur? SelectedUtilisateur { get; set; }
    [ObservableProperty] public partial bool HasResult { get; set; }

    public ObservableCollection<Lieu> Villes { get; } = [];
    public ObservableCollection<Utilisateur> Utilisateurs { get; } = [];
    public ObservableCollection<Trajet> ItineraireCalcule { get; } = [];

    public AddVoyageViewModel(VoyageService voyageService, TrajetService trajetService, LieuService lieuService, UtilisateurService utilisateurService)
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