using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public class AccountViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    private string _fullName = string.Empty;
    private string _email = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AccountViewModel(UserService userService)
    {
        _userService = userService;
        LogoutCommand = new Command(async () => await OnLogout());
        LoadUser();
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
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand LogoutCommand { get; }

    public void LoadUser()
    {
        var user = _userService.GetCurrentUser();
        if (user is not null)
        {
            FullName = user.FullName;
            Email = user.Email;
        }
    }

    private async Task OnLogout()
    {
        _userService.Logout();
        await Shell.Current.GoToAsync("//AuthenticationPage");
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
