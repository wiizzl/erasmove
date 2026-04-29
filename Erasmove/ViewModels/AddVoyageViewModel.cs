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

    [RelayCommand]
    public async Task CalculerItineraireAsync()
    {
        if (SelectedDepart == null || SelectedArrivee == null)
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Veuillez sélectionner les villes de départ et d'arrivée.", "OK");
            return;
        }

        if (SelectedDepart.Id == SelectedArrivee.Id)
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Les villes de départ et d'arrivée doivent être différentes.", "OK");
            return;
        }

        ItineraireCalcule.Clear();
        HasResult = false;

        try
        {
            IsBusy = true;
            var path = await _trajetService.FindBestPathAsync(SelectedDepart.Id, SelectedArrivee.Id);

            if (path != null)
            {
                foreach (var trajet in path)
                    ItineraireCalcule.Add(trajet);

                HasResult = true;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Info", "Aucun itinéraire trouvé entre ces deux villes.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erreur", $"Impossible de calculer l'itinéraire : {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override bool ValidateForm()
    {
        return SelectedUtilisateur != null && HasResult && ItineraireCalcule.Count > 0;
    }

    protected override async Task ExecuteSaveAsync()
    {
        var libelle = $"{SelectedDepart!.Ville} → {SelectedArrivee!.Ville}";
        await _voyageService.SaveItineraireCalculeAsync(libelle, SelectedUtilisateur!.Id, [.. ItineraireCalcule]);
    }
}