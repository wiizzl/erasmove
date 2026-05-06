using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models.Interfaces;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels.Base;

public abstract partial class BaseCatalogViewModel<T> : ObservableObject where T : class, IEntity
{
    private readonly ICrudService<T> _service;
    private readonly INavigationService _navigationService;
    private readonly string _routeAjout;

    public ObservableCollection<T> Items { get; } = [];

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    protected BaseCatalogViewModel(ICrudService<T> service, INavigationService navigationService, string routeAjout)
    {
        _service = service;
        _navigationService = navigationService;
        _routeAjout = routeAjout;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public virtual async Task LoadItemsAsync()
    {
        if (IsRefreshing)
        {
            return;
        }

        try
        {
            IsRefreshing = true;
            var result = await _service.GetAllAsync();

            Items.Clear();
            foreach (var item in result)
            {
                Items.Add(item);
            }
        }
        catch
        {
            await _navigationService.DisplayAlertAsync("Erreur", "Impossible de charger les données.", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    protected virtual async Task DeleteItemAsync(T item)
    {
        var confirm = await _navigationService.DisplayAlertAsync("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
        if (!confirm)
        {
            return;
        }

        try
        {
            await _service.DeleteAsync(item.Id);
            Items.Remove(item);
        }
        catch (Exception)
        {
            await _navigationService.DisplayAlertAsync("Erreur", "Suppression impossible de cet élément.", "OK");
        }
    }

    [RelayCommand]
    protected virtual async Task EditItemAsync(T item)
    {
        var parameters = new Dictionary<string, object>
        {
            [BaseAddViewModel.EditingItemParameterName] = item
        };

        await _navigationService.GoToAsync(_routeAjout, parameters);
    }

    [RelayCommand]
    protected virtual async Task GoToAddPageAsync()
    {
        await _navigationService.GoToAsync(_routeAjout);
    }
}
