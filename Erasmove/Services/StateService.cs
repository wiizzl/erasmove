using Erasmove.Models.Interfaces;
using Erasmove.Services.Interfaces;

namespace Erasmove.Services;

public class StateService : IStateService
{
    private IEntity? _editingItem;

    public void SetEditingItem(IEntity? item)
    {
        _editingItem = item;
    }

    public IEntity? GetEditingItem()
    {
        return _editingItem;
    }

    public void ClearEditingItem()
    {
        _editingItem = null;
    }
}
