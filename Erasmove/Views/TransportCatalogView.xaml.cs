using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class TransportCatalogView : ContentPage
{
    public TransportCatalogView(TransportCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TransportCatalogViewModel vm)
        {
            vm.LoadItemsCommand.Execute(null);
        }
    }
}