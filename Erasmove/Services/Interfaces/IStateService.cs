using Erasmove.Models;
using Erasmove.Models.Interfaces;

namespace Erasmove.Services.Interfaces;

public interface IStateService
{
    void SetEditingItem(IEntity? item);
    IEntity? GetEditingItem();
    void ClearEditingItem();
    Utilisateur? CurrentUser { get; set; }
}
