using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUtilisateurService _utilisateurService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] public partial string Login { get; set; } = string.Empty;
    [ObservableProperty] public partial string Password { get; set; } = string.Empty;
    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty] public partial bool IsBusy { get; set; }

    public LoginViewModel(IUtilisateurService utilisateurService, INavigationService navigationService)
    {
        _utilisateurService = utilisateurService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez saisir vos identifiants.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var user = await _utilisateurService.AuthenticateAsync(Login, Password);

            if (user != null)
            {
                if (user.IsAdmin)
                {
                    await _navigationService.GoToAsync("//Admin");
                }
                else
                {
                    await _navigationService.GoToAsync("//Home");
                }
            }
            else
            {
                ErrorMessage = "Identifiant ou mot de passe incorrect.";
            }
        }
        catch
        {
            ErrorMessage = "Erreur de connexion au serveur.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}