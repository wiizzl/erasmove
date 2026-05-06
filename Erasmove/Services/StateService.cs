using Erasmove.Models;
using Erasmove.Models.Interfaces;
using Erasmove.Services.Interfaces;

namespace Erasmove.Services;

public class StateService : IStateService
{
    private IEntity? _editingItem;

    public Utilisateur? CurrentUser { get; set; }

    public void SetEditingItem(IEntity? item) => _editingItem = item;
    public IEntity? GetEditingItem() => _editingItem;
    public void ClearEditingItem() => _editingItem = null;
}
