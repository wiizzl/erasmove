using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Erasmove.Models;
using Erasmove.Models.Interfaces;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class AddVoyageViewModel : BaseAddViewModel
{
    private readonly IVoyageService _voyageService;
    private readonly ITrajetService _trajetService;
    private readonly ILieuService _lieuService;
    private readonly IUtilisateurService _utilisateurService;
    private bool _referenceDataLoaded;
    private bool _isLoadingData;

    [ObservableProperty] public partial string Libelle { get; set; } = string.Empty;
    [ObservableProperty] public partial Lieu? SelectedDepart { get; set; }
    [ObservableProperty] public partial Lieu? SelectedArrivee { get; set; }
    [ObservableProperty] public partial Utilisateur? SelectedUtilisateur { get; set; }
    [ObservableProperty] public partial bool HasResult { get; set; }
    [ObservableProperty] public partial string ItineraireMessage { get; set; } = string.Empty;

    public ObservableCollection<Lieu> Lieux { get; } = [];
    public ObservableCollection<Utilisateur> Utilisateurs { get; } = [];
    public ObservableCollection<Trajet> ItineraireCalcule { get; } = [];

    public AddVoyageViewModel(IVoyageService voyageService, ITrajetService trajetService, ILieuService lieuService, IUtilisateurService utilisateurService, INavigationService navigationService) : base(navigationService)
    {
        _voyageService = voyageService;
        _trajetService = trajetService;
        _lieuService = lieuService;
        _utilisateurService = utilisateurService;
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

            var lieux = await _lieuService.GetAllAsync();
            var utilisateurs = await _utilisateurService.GetAllAsync();

            Lieux.Clear();
            foreach (var lieu in lieux)
            {
                Lieux.Add(lieu);
            }

            Utilisateurs.Clear();
            foreach (var u in utilisateurs.Where(user => user.RoleLibelle.Equals("Voyageur", StringComparison.OrdinalIgnoreCase)))
            {
                Utilisateurs.Add(u);
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

    [RelayCommand]
    public async Task CalculateBestItineraryAsync()
    {
        ItineraireCalcule.Clear();
        HasResult = false;

        if (SelectedDepart == null || SelectedArrivee == null || SelectedDepart.Id == SelectedArrivee.Id)
        {
            ItineraireMessage = "Sélectionnez un lieu de départ et un lieu d'arrivée différents.";
            return;
        }

        var trajets = await _trajetService.GetAllAsync();
        var bestPath = FindShortestPathBySegments(SelectedDepart.Id, SelectedArrivee.Id, trajets);

        if (bestPath.Count == 0)
        {
            ItineraireMessage = "Aucun itinéraire trouvé entre les lieux sélectionnés.";
            return;
        }

        foreach (var trajet in bestPath)
        {
            ItineraireCalcule.Add(trajet);
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Itinéraire trouvé: {bestPath.Count} étape(s)");
        for (var i = 0; i < bestPath.Count; i++)
        {
            var t = bestPath[i];
            sb.AppendLine($"{i + 1}. {t.LieuDepartNom} -> {t.LieuArriveeNom} ({t.TypeTransportLibelle} - {t.CompagnieTransport})");
        }

        ItineraireMessage = sb.ToString().TrimEnd();
        HasResult = true;
    }

    protected override bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(Libelle) || SelectedUtilisateur == null)
        {
            return false;
        }

        if (EditingItem != null)
        {
            return true;
        }

        return SelectedDepart != null &&
               SelectedArrivee != null &&
               SelectedDepart.Id != SelectedArrivee.Id &&
               ItineraireCalcule.Count > 0;
    }

    protected override async Task ExecuteSaveAsync()
    {
        await _voyageService.CreateVoyageWithEtapesAsync(Libelle, SelectedUtilisateur!.Id, ItineraireCalcule.ToList());
    }

    protected override Task ExecuteUpdateAsync()
    {
        throw new NotImplementedException();
    }

    protected override void LoadItemData(IEntity item)
    {
        throw new NotImplementedException();
    }

    partial void OnSelectedDepartChanged(Lieu? value)
    {
        InvalidateComputedItineraryIfNeeded();
    }

    partial void OnSelectedArriveeChanged(Lieu? value)
    {
        InvalidateComputedItineraryIfNeeded();
    }

    private void InvalidateComputedItineraryIfNeeded()
    {
        if (EditingItem != null)
        {
            return;
        }

        ItineraireCalcule.Clear();
        HasResult = false;
        ItineraireMessage = string.Empty;
    }

    private static List<Trajet> FindShortestPathBySegments(int departId, int arriveeId, List<Trajet> trajets)
    {
        var adjacency = trajets
            .GroupBy(trajet => trajet.LieuDepartId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var queue = new Queue<int>();
        var visited = new HashSet<int> { departId };
        var predecessors = new Dictionary<int, (int PreviousLieuId, Trajet ViaTrajet)>();

        queue.Enqueue(departId);

        while (queue.Count > 0)
        {
            var currentLieuId = queue.Dequeue();

            if (!adjacency.TryGetValue(currentLieuId, out var outgoing))
            {
                continue;
            }

            foreach (var trajet in outgoing)
            {
                var nextLieuId = trajet.LieuArriveeId;
                if (!visited.Add(nextLieuId))
                {
                    continue;
                }

                predecessors[nextLieuId] = (currentLieuId, trajet);

                if (nextLieuId == arriveeId)
                {
                    return RebuildPath(departId, arriveeId, predecessors);
                }

                queue.Enqueue(nextLieuId);
            }
        }

        return [];
    }

    private static List<Trajet> RebuildPath(int departId, int arriveeId, Dictionary<int, (int PreviousLieuId, Trajet ViaTrajet)> predecessors)
    {
        var path = new List<Trajet>();
        var currentLieuId = arriveeId;

        while (currentLieuId != departId)
        {
            if (!predecessors.TryGetValue(currentLieuId, out var predecessor))
            {
                return [];
            }

            path.Add(predecessor.ViaTrajet);
            currentLieuId = predecessor.PreviousLieuId;
        }

        path.Reverse();

        return path;
    }
}