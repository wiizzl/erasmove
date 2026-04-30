using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddTrajetView : ContentPage
{
    private readonly IStateService _stateService;

    public AddTrajetView(AddTrajetViewModel viewModel, IStateService stateService)
    {
        InitializeComponent();
        _stateService = stateService;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is not AddTrajetViewModel viewModel)
        {
            return;
        }
        
        var item = _stateService.GetEditingItem() as Trajet;
        viewModel.SetEditingItem(item);
        _stateService.ClearEditingItem();
    }
}