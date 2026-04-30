using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddLieuView : ContentPage
{
    private readonly IStateService _stateService;

    public AddLieuView(AddLieuViewModel viewModel, IStateService stateService)
    {
        InitializeComponent();
        _stateService = stateService;
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is not AddLieuViewModel viewModel)
        {
            return;
        }
        
        var item = _stateService.GetEditingItem() as Lieu;
        viewModel.SetEditingItem(item);
        _stateService.ClearEditingItem();
    }
}