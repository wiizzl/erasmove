namespace Erasmove.Models;

public class VoyageEtapeDetail
{
    public int Ordre { get; set; }
    public string NomDepart { get; set; } = string.Empty;
    public double LatDepart { get; set; }
    public double LonDepart { get; set; }
    public string NomArrivee { get; set; } = string.Empty;
    public double LatArrivee { get; set; }
    public double LonArrivee { get; set; }
    public string CompagnieTransport { get; set; } = string.Empty;
    public string TypeTransportLibelle { get; set; } = string.Empty;
}