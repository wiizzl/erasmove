namespace Erasmove.Models;

public class Trajet : IEntity
{
    public int Id { get; set; }
    public int LieuDepartId { get; set; }
    public int LieuArriveeId { get; set; }
    public int TransportId { get; set; }
    public string VilleDepart { get; set; } = string.Empty;
    public string PaysDepart { get; set; } = string.Empty;
    public string VilleArrivee { get; set; } = string.Empty;
    public string PaysArrivee { get; set; } = string.Empty;
    public string CompagnieTransport { get; set; } = string.Empty;
    public string TypeTransportLibelle { get; set; } = string.Empty;
}