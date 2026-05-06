using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models.Interfaces;
using Erasmove.Services;

namespace Erasmove.ViewModels.Base;

public abstract partial class BaseAddViewModel : ObservableObject, IQueryAttributable
{
    public const string EditingItemParameterName = "editingItem";

    private readonly INavigationService _navigationService;
    protected IEntity? EditingItem { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Ajouter";

    protected BaseAddViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected virtual bool LoadItemDataImmediately => true;
    protected virtual bool HasReferenceDataLoaded => true;

    public virtual void SetEditingItem(IEntity? item)
    {
        EditingItem = item;
        PageTitle = item != null ? "Modifier" : "Ajouter";
        if (item != null && (LoadItemDataImmediately || HasReferenceDataLoaded))
        {
            LoadItemData(item);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(EditingItemParameterName, out var editingItem) && editingItem is IEntity entity)
        {
            SetEditingItem(entity);
            return;
        }

        SetEditingItem(null);
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!ValidateForm())
        {
            await _navigationService.DisplayAlertAsync("Erreur", "Validation des champs invalide.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            if (EditingItem != null)
            {
                await ExecuteUpdateAsync();
            }
            else
            {
                await ExecuteSaveAsync();
            }

            await _navigationService.GoBackAsync();
        }
        catch
        {
            var message = EditingItem != null
                ? "Impossible de modifier l'élément."
                : "Impossible d'ajouter l'élément.";
            await _navigationService.DisplayAlertAsync("Erreur", message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }

    protected abstract Task ExecuteSaveAsync();
    protected abstract Task ExecuteUpdateAsync();
    protected abstract bool ValidateForm();
    protected abstract void LoadItemData(IEntity item);
}
