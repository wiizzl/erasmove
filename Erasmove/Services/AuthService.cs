using Erasmove.Models;

namespace Erasmove.Services;

public class AuthService
{
    public Account? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;
    public bool IsManager => CurrentUser?.RoleId == 2; 

    public void Login(Account account)
    {
        CurrentUser = account;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}