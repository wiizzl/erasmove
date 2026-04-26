using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class TrajetCatalogView : ContentPage
{
    public TrajetCatalogView(TrajetCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TrajetCatalogViewModel vm)
        {
            vm.LoadItemsCommand.Execute(null);
        }
    }
}