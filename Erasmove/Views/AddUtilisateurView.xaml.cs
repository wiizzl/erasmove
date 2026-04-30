using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddUtilisateurView : ContentPage
{
    private readonly IStateService _stateService;

    public AddUtilisateurView(AddUtilisateurViewModel viewModel, IStateService stateService)
    {
        InitializeComponent();
        _stateService = stateService;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddUtilisateurViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is not AddUtilisateurViewModel viewModel)
        {
            return;
        }
        
        var item = _stateService.GetEditingItem() as Utilisateur;
        viewModel.SetEditingItem(item);
        _stateService.ClearEditingItem();
    }
}