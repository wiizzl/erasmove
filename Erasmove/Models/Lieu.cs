namespace Erasmove.Models;

public class Lieu : IEntity
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string Pays { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}