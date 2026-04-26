using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Erasmove.ViewModels;

public abstract partial class BaseAddViewModel : ObservableObject
{
    [ObservableProperty] 
    public partial bool IsBusy { get; set; }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!ValidateForm())
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Validation des champs invalide.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            
            await ExecuteSaveAsync();
            await Shell.Current.GoToAsync(".."); 
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible d'ajouter l'élément.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    protected abstract Task ExecuteSaveAsync();
    protected abstract bool ValidateForm();
}