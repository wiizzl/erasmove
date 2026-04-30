using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddTransportView : ContentPage
{
    private readonly IStateService _stateService;

    public AddTransportView(AddTransportViewModel viewModel, IStateService stateService)
    {
        InitializeComponent();
        _stateService = stateService;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is AddTransportViewModel viewModel)
        {
            var item = _stateService.GetEditingItem() as Transport;
            viewModel.SetEditingItem(item);
            _stateService.ClearEditingItem();
        }
    }
}