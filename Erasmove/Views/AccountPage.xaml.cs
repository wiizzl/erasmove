using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AccountPage : ContentPage
{
    private readonly AccountViewModel _viewModel;

    public AccountPage(AccountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUserAsync();
    }
}
