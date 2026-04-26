using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class VoyageCatalogView : ContentPage
{
    public VoyageCatalogView(VoyageCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is VoyageCatalogViewModel vm)
        {
            vm.LoadItemsCommand.Execute(null);
        }
    }
}