using Erasmove.Models;

namespace Erasmove.Services;

public class CurrentUserService
{
    private Utilisateur? _currentUser;

    public Utilisateur? CurrentUser => _currentUser;

    public void SetCurrentUser(Utilisateur user)
    {
        _currentUser = user;
    }

    public void ClearCurrentUser()
    {
        _currentUser = null;
    }

    public bool IsUserLoggedIn => _currentUser != null;
}
