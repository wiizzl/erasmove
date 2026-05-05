using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Models.Interfaces;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class AddUtilisateurViewModel : BaseAddViewModel
{
    private readonly IUtilisateurService _utilisateurService;
    private bool _referenceDataLoaded;

    [ObservableProperty] public partial string Nom { get; set; } = string.Empty;
    [ObservableProperty] public partial string Prenom { get; set; } = string.Empty;
    [ObservableProperty] public partial string Login { get; set; } = string.Empty;
    [ObservableProperty] public partial string MotDePasse { get; set; } = string.Empty;
    [ObservableProperty] public partial Role? SelectedRole { get; set; }

    public ObservableCollection<Role> Roles { get; } = [];

    public AddUtilisateurViewModel(IUtilisateurService utilisateurService, INavigationService navigationService) : base(
        navigationService)
    {
        _utilisateurService = utilisateurService;
    }

    protected override bool LoadItemDataImmediately => false;
    protected override bool HasReferenceDataLoaded => _referenceDataLoaded;

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var roles = await _utilisateurService.GetRolesAsync();
        Roles.Clear();
        foreach (var role in roles)
        {
            Roles.Add(role);
        }

        _referenceDataLoaded = true;

        if (EditingItem is not null)
        {
            LoadItemData(EditingItem);
        }
    }

    protected override bool ValidateForm()
    {
        return !string.IsNullOrWhiteSpace(Nom) &&
               !string.IsNullOrWhiteSpace(Prenom) &&
               !string.IsNullOrWhiteSpace(Login) &&
               (EditingItem != null || !string.IsNullOrWhiteSpace(MotDePasse)) &&
               SelectedRole != null;
    }

    protected override async Task ExecuteSaveAsync()
    {
        var u = new Utilisateur
        {
            Nom = Nom,
            Prenom = Prenom,
            Login = Login,
            RoleId = SelectedRole!.Id
        };

        await _utilisateurService.AddUtilisateurAsync(u, MotDePasse);
    }

    protected override async Task ExecuteUpdateAsync()
    {
        if (EditingItem is not Utilisateur utilisateur) return;

        utilisateur.Nom = Nom;
        utilisateur.Prenom = Prenom;
        utilisateur.Login = Login;
        utilisateur.RoleId = SelectedRole!.Id;

        await _utilisateurService.UpdateUtilisateurAsync(utilisateur,
            string.IsNullOrEmpty(MotDePasse) ? null : MotDePasse);
    }

    protected override void LoadItemData(IEntity item)
    {
        if (item is not Utilisateur utilisateur)
        {
            return;
        }

        Nom = utilisateur.Nom;
        Prenom = utilisateur.Prenom;
        Login = utilisateur.Login;
        SelectedRole = Roles.FirstOrDefault(role => role.Id == utilisateur.RoleId);
        MotDePasse = string.Empty;
    }
}