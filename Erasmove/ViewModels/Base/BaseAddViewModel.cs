using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Services;

namespace Erasmove.ViewModels.Base;

public abstract partial class BaseAddViewModel : ObservableObject
{
    private readonly INavigationService NavigationService;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    protected BaseAddViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!ValidateForm())
        {
            await NavigationService.DisplayAlertAsync("Erreur", "Validation des champs invalide.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            await ExecuteSaveAsync();
            await NavigationService.GoBackAsync();
        }
        catch
        {
            await NavigationService.DisplayAlertAsync("Erreur", "Impossible d'ajouter l'élément.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await NavigationService.GoBackAsync();
    }

    protected abstract Task ExecuteSaveAsync();
    protected abstract bool ValidateForm();
}
