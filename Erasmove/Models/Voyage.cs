namespace Erasmove.Models;

public class Voyage : IEntity
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public int UtilisateurId { get; set; }
    public DateTime DateCreation { get; set; }
    public string UtilisateurNom { get; set; } = string.Empty;
    public string UtilisateurPrenom { get; set; } = string.Empty;
}