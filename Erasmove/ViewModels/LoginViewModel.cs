using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Repositories;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AccountRepository _accountRepository;
    private readonly AuthService _authService;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    public LoginViewModel(AccountRepository accountRepository, AuthService authService)
    {
        _accountRepository = accountRepository;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez renseigner votre email et votre mot de passe.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var account = await _accountRepository.GetAccountByEmailAsync(Email);
            
            if (account != null && account.Password == Password) 
            {
                _authService.Login(account);
                
                await Shell.Current.GoToAsync($"//MainPage");
                
                Email = string.Empty;
                Password = string.Empty;
            }
            else
            {
                ErrorMessage = "Email ou mot de passe incorrect.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur de connexion à la base de données.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}