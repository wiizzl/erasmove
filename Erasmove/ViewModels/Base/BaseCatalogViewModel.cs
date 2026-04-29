using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels.Base;

public abstract partial class BaseCatalogViewModel<T> : ObservableObject where T : class, IEntity
{
    protected readonly ICrudService<T> Service;
    protected readonly INavigationService NavigationService;
    private readonly string _routeAjout;

    public ObservableCollection<T> Items { get; } = [];

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    protected BaseCatalogViewModel(ICrudService<T> service, INavigationService navigationService, string routeAjout)
    {
        Service = service;
        NavigationService = navigationService;
        _routeAjout = routeAjout;
    }

    [RelayCommand]
    public virtual async Task LoadItemsAsync()
    {
        try
        {
            IsRefreshing = true;
            var result = await Service.GetAllAsync();

            Items.Clear();
            foreach (var item in result)
            {
                Items.Add(item);
            }
        }
        catch
        {
            await NavigationService.DisplayAlertAsync("Erreur", "Impossible de charger les données.", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    protected virtual async Task DeleteItemAsync(T item)
    {
        var confirm = await NavigationService.DisplayAlertAsync("Confirmation", "Supprimer cet élément ?", "Oui", "Non");
        if (!confirm)
        {
            return;
        }

        try
        {
            await Service.DeleteAsync(item.Id);
            Items.Remove(item);
        }
        catch (Exception)
        {
            await NavigationService.DisplayAlertAsync("Erreur", "Suppression impossible de cet élément.", "OK");
        }
    }

    [RelayCommand]
    protected virtual async Task GoToAddPageAsync()
    {
        await NavigationService.GoToAsync(_routeAjout);
    }
}
