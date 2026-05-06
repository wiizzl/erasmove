using Erasmove.Services.Interfaces;
using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddVoyageView : ContentPage
{
    private readonly IStateService _stateService;

    public AddVoyageView(AddVoyageViewModel viewModel, IStateService stateService)
    {
        InitializeComponent();
        _stateService = stateService;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is not AddVoyageViewModel viewModel)
        {
            return;
        }

        // Modification désactivée : on n'applique jamais d'élément en édition.
        _stateService.ClearEditingItem();
    }
}