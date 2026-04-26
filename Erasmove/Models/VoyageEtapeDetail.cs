namespace Erasmove.Models;

public class VoyageEtapeDetail
{
    public int Ordre { get; set; }
    public string VilleDepart { get; set; } = string.Empty;
    public string VilleArrivee { get; set; } = string.Empty;
    public string CompagnieTransport { get; set; } = string.Empty;
    public string TypeTransportLibelle { get; set; } = string.Empty;
}