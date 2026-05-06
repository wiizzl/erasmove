using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddTransportView : ContentPage
{
    public AddTransportView(AddTransportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddTransportViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}