using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Models.Interfaces;
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

    public ObservableCollection<Lieu> Lieux { get; } = [];
    public ObservableCollection<Utilisateur> Utilisateurs { get; } = [];

    public AddVoyageViewModel(IVoyageService voyageService, ITrajetService trajetService, ILieuService lieuService,
        IUtilisateurService utilisateurService, INavigationService navigationService) : base(navigationService)
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

        Lieux.Clear();
        foreach (var lieu in lieux)
        {
            Lieux.Add(lieu);
        }

        Utilisateurs.Clear();
        foreach (var u in utilisateurs)
        {
            Utilisateurs.Add(u);
        }

        if (EditingItem is not null)
        {
            LoadItemData(EditingItem);
        }
    }

    protected override bool ValidateForm()
    {
        return SelectedUtilisateur != null &&
               SelectedDepart != null &&
               SelectedArrivee != null &&
               SelectedDepart.Id != SelectedArrivee.Id;
    }

    protected override async Task ExecuteSaveAsync()
    {

    }

    protected override async Task ExecuteUpdateAsync()
    {
        
    }

    protected override void LoadItemData(IEntity item)
    {
        if (item is not Voyage voyage)
        {
            return;
        }
        
        SelectedUtilisateur = Utilisateurs.FirstOrDefault(utilisateur => utilisateur.Id == voyage.UtilisateurId);
    }
}