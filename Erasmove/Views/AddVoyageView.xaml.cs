using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddVoyageView : ContentPage
{
    public AddVoyageView(AddVoyageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddVoyageViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}