using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class VoyageCatalogViewModel : BaseCatalogViewModel<Voyage>
{
    private readonly VoyageService _voyageService;

    public VoyageCatalogViewModel(VoyageService service) : base(service, "AddVoyage")
    {
        _voyageService = service;
    }

    public override async Task LoadItemsAsync()
    {
        try
        {
            IsRefreshing = true;
            var voyages = await Service.GetAllAsync();

            Items.Clear();
            foreach (var voyage in voyages)
            {
                voyage.Etapes = await _voyageService.GetItineraireVoyageAsync(voyage.Id);
                Items.Add(voyage);
            }
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Impossible de charger les données.", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
