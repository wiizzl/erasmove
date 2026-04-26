using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddUtilisateurView : ContentPage
{
    public AddUtilisateurView(AddUtilisateurViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddUtilisateurViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}