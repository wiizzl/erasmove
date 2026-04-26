using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class AddUtilisateurViewModel : BaseAddViewModel
{
    private readonly UtilisateurService _utilisateurService;

    [ObservableProperty] public partial string Nom { get; set; } = string.Empty;
    [ObservableProperty] public partial string Prenom { get; set; } = string.Empty;
    [ObservableProperty] public partial string Login { get; set; } = string.Empty;
    [ObservableProperty] public partial string MotDePasse { get; set; } = string.Empty;
    [ObservableProperty] public partial Role? SelectedRole { get; set; }

    public ObservableCollection<Role> Roles { get; } = [];

    public AddUtilisateurViewModel(UtilisateurService utilisateurService)
    {
        _utilisateurService = utilisateurService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var roles = await _utilisateurService.GetRolesAsync();
        Roles.Clear();
        foreach (var role in roles)
        {
            Roles.Add(role);
        }
    }

    protected override bool ValidateForm()
    {
        return !string.IsNullOrWhiteSpace(Nom) &&
               !string.IsNullOrWhiteSpace(Prenom) &&
               !string.IsNullOrWhiteSpace(Login) &&
               !string.IsNullOrWhiteSpace(MotDePasse) &&
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
}