using Erasmove.Models.Interfaces;

namespace Erasmove.Models;

public class Trajet : IEntity
{
    public int Id { get; set; }
    public int LieuDepartId { get; set; }
    public int LieuArriveeId { get; set; }
    public int TransportId { get; set; }
    public string LieuDepartNom { get; set; } = string.Empty;
    public string LieuArriveeNom { get; set; } = string.Empty;
    public string CompagnieTransport { get; set; } = string.Empty;
    public string TypeTransportLibelle { get; set; } = string.Empty;
}