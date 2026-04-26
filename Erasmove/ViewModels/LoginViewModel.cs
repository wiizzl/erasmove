using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly UtilisateurService _utilisateurService;

    [ObservableProperty] public partial string Login { get; set; } = string.Empty;
    [ObservableProperty] public partial string Password { get; set; } = string.Empty;
    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty] public partial bool IsBusy { get; set; }

    public LoginViewModel(UtilisateurService utilisateurService)
    {
        _utilisateurService = utilisateurService;
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
                    await Shell.Current.GoToAsync("//Admin");
                }
                else
                {
                    await Shell.Current.GoToAsync("//Home");
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