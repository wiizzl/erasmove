using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddTrajetView : ContentPage
{
    public AddTrajetView(AddTrajetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddTrajetViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}