using CommunityToolkit.Mvvm.ComponentModel;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AuthService _authService;

    public MainViewModel(AuthService authService)
    {
        _authService = authService;
    }
}