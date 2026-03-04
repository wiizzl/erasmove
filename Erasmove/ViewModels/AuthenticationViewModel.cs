using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public class AuthenticationViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    private bool _isLoginMode = true;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _fullName = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AuthenticationViewModel(UserService userService)
    {
        _userService = userService;
        LoginCommand = new Command(async () => await OnLogin(), () => !IsLoading);
        RegisterCommand = new Command(async () => await OnRegister(), () => !IsLoading);
        ToggleModeCommand = new Command(OnToggleMode);
    }

    public bool IsLoginMode
    {
        get => _isLoginMode;
        set
        {
            if (_isLoginMode != value)
            {
                _isLoginMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRegisterMode));
                OnPropertyChanged(nameof(ModeButtonText));
                OnPropertyChanged(nameof(ActionButtonText));
                ClearForm();
            }
        }
    }

    public bool IsRegisterMode => !IsLoginMode;

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (_confirmPassword != value)
            {
                _confirmPassword = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (_fullName != value)
            {
                _fullName = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }
    }

    public string ModeButtonText => IsLoginMode ? "Créer un compte" : "J'ai déjà un compte";
    public string ActionButtonText => IsLoginMode ? "Se connecter" : "S'inscrire";

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand ToggleModeCommand { get; }

    private async Task OnLogin()
    {
        if (!ValidateLoginForm())
            return;

        IsLoading = true;

        try
        {
            await Task.Delay(300);

            if (!_userService.EmailExists(Email))
            {
                ErrorMessage = "Aucun compte n'existe avec cet email";
                return;
            }

            var user = _userService.Login(Email, Password);
            if (user is null)
            {
                ErrorMessage = "Mot de passe incorrect";
                return;
            }

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur de connexion : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnRegister()
    {
        if (!ValidateRegisterForm())
            return;

        IsLoading = true;

        try
        {
            await Task.Delay(300);

            if (_userService.EmailExists(Email))
            {
                ErrorMessage = "Un compte existe déjà avec cet email";
                return;
            }

            _userService.Register(FullName, Email, Password);

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur d'inscription : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OnToggleMode()
    {
        IsLoginMode = !IsLoginMode;
    }

    private bool ValidateLoginForm()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Veuillez entrer votre email";
            return false;
        }

        if (!IsValidEmail(Email))
        {
            ErrorMessage = "Email invalide";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez entrer votre mot de passe";
            return false;
        }

        return true;
    }

    private bool ValidateRegisterForm()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Veuillez entrer votre nom complet";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Veuillez entrer votre email";
            return false;
        }

        if (!IsValidEmail(Email))
        {
            ErrorMessage = "Email invalide";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez entrer un mot de passe";
            return false;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères";
            return false;
        }

        if (string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Veuillez confirmer votre mot de passe";
            return false;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Les mots de passe ne correspondent pas";
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void ClearForm()
    {
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        FullName = string.Empty;
        ErrorMessage = string.Empty;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
