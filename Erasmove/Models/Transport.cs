using Erasmove.Models.Interfaces;

namespace Erasmove.Models;

public class Transport : IEntity
{
    public int Id { get; set; }
    public string Compagnie { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string TypeLibelle { get; set; } = string.Empty;
}