namespace Erasmove.Models;

public class Trip
{
    public int Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    
    public int TravelerId { get; set; }
    public int PlaceId { get; set; }
    public int TransportId { get; set; }

    public Traveler? Traveler { get; set; }
    public Place? Place { get; set; }
    public Transport? Transport { get; set; }
}