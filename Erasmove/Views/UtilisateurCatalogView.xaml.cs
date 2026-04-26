using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class UtilisateurCatalogView : ContentPage
{
    public UtilisateurCatalogView(UtilisateurCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UtilisateurCatalogViewModel vm)
        {
            vm.LoadItemsCommand.Execute(null);
        }
    }
}