namespace Erasmove.Models;

public class Trip
{
    public int Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    
    public int TravelerId { get; set; }
    public int PlaceId { get; set; }
    public int TransportId { get; set; }

    public Traveler Traveler { get; set; }
    public Place Place { get; set; }
    public Transport Transport { get; set; }

    // Nouveaux champs pour affichage des détails
    public string StartCity { get; set; } = string.Empty;
    public string EndCity { get; set; } = string.Empty;
    public string DurationText { get; set; } = string.Empty;
    public string TripName { get; set; } = string.Empty;
    public string WaypointsJson { get; set; } = string.Empty;
}