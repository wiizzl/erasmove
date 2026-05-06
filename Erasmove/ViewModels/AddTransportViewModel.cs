using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Models.Interfaces;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class AddTransportViewModel : BaseAddViewModel
{
    private readonly ITransportService _transportService;
    private bool _referenceDataLoaded;
    private bool _isLoadingData;

    [ObservableProperty] public partial string Compagnie { get; set; } = string.Empty;
    [ObservableProperty] public partial TypeTransport? SelectedType { get; set; }

    public ObservableCollection<TypeTransport> Types { get; } = [];

    public AddTransportViewModel(ITransportService transportService, INavigationService navigationService) : base(navigationService)
    {
        _transportService = transportService;
    }

    protected override bool LoadItemDataImmediately => false;
    protected override bool HasReferenceDataLoaded => _referenceDataLoaded;

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task LoadDataAsync()
    {
        if (_referenceDataLoaded || _isLoadingData)
        {
            return;
        }

        try
        {
            _isLoadingData = true;

            var types = await _transportService.GetTypeTransportsAsync();
            Types.Clear();
            foreach (var type in types)
            {
                Types.Add(type);
            }

            _referenceDataLoaded = true;

            if (EditingItem is not null)
            {
                LoadItemData(EditingItem);
            }
        }
        finally
        {
            _isLoadingData = false;
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

    protected override async Task ExecuteUpdateAsync()
    {
        if (EditingItem is not Transport transport)
        {
            return;
        };

        transport.Compagnie = Compagnie;
        transport.TypeId = SelectedType!.Id;

        await _transportService.UpdateTransportAsync(transport);
    }

    protected override void LoadItemData(IEntity item)
    {
        if (item is not Transport transport)
        {
            return;
        }

        Compagnie = transport.Compagnie;
        SelectedType = Types.FirstOrDefault(type => type.Id == transport.TypeId);
    }
}