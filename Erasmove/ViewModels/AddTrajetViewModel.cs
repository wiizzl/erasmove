using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class AddTrajetViewModel : BaseAddViewModel
{
    private readonly ITrajetService _trajetService;
    private readonly ILieuService _lieuService;
    private readonly ITransportService _transportService;

    [ObservableProperty] public partial Lieu? SelectedDepart { get; set; }
    [ObservableProperty] public partial Lieu? SelectedArrivee { get; set; }
    [ObservableProperty] public partial Transport? SelectedTransport { get; set; }

    public ObservableCollection<Lieu> Lieux { get; } = [];
    public ObservableCollection<Transport> Transports { get; } = [];

    public AddTrajetViewModel(ITrajetService trajetService, ILieuService lieuService, ITransportService transportService, INavigationService navigationService) : base(navigationService)
    {
        _trajetService = trajetService;
        _lieuService = lieuService;
        _transportService = transportService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var lieux = await _lieuService.GetAllAsync();
        var transports = await _transportService.GetAllAsync();

        Lieux.Clear();
        foreach (var lieu in lieux)
        {
            Lieux.Add(lieu);
        }

        Transports.Clear();
        foreach (var transport in transports)
        {
            Transports.Add(transport);
        }
    }

    protected override bool ValidateForm()
    {
        return SelectedDepart != null &&
               SelectedArrivee != null &&
               SelectedTransport != null &&
               SelectedDepart.Id != SelectedArrivee.Id;
    }

    protected override async Task ExecuteSaveAsync()
    {
        var trajet = new Trajet
        {
            LieuDepartId = SelectedDepart!.Id,
            LieuArriveeId = SelectedArrivee!.Id,
            TransportId = SelectedTransport!.Id
        };

        await _trajetService.AddTrajetAsync(trajet);
    }
}