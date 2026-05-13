using Erasmove.Models.Interfaces;

namespace Erasmove.Models;

public class Utilisateur : IEntity
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleLibelle { get; set; } = string.Empty;
    public bool IsAdmin => RoleLibelle.Equals("Gestionnaire");
}