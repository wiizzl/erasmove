using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class LieuCatalogView : ContentPage
{
    public LieuCatalogView(LieuCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LieuCatalogViewModel vm)
        {
            vm.LoadItemsCommand.Execute(null);
        }
    }
}