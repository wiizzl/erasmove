using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class AddTransportViewModel : BaseAddViewModel
{
    private readonly ITransportService _transportService;

    [ObservableProperty] public partial string Compagnie { get; set; } = string.Empty;
    [ObservableProperty] public partial TypeTransport? SelectedType { get; set; }

    public ObservableCollection<TypeTransport> Types { get; } = [];

    public AddTransportViewModel(ITransportService transportService, INavigationService navigationService) : base(navigationService)
    {
        _transportService = transportService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var types = await _transportService.GetTypeTransportsAsync();
        Types.Clear();
        foreach (var type in types)
        {
            Types.Add(type);
        }
    }

    protected override bool ValidateForm()
    {
        return !string.IsNullOrWhiteSpace(Compagnie) && SelectedType != null;
    }

    protected override async Task ExecuteSaveAsync()
    {
        var transport = new Transport
        {
            Compagnie = Compagnie,
            TypeId = SelectedType!.Id
        };

        await _transportService.AddTransportAsync(transport);
    }
}