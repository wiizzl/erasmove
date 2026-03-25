using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly AuthService _authService;

    public AppShellViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task LogoutAsync()
    {
        _authService.Logout();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}