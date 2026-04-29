using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface IUtilisateurService : ICrudService<Utilisateur>
{
    Task<Utilisateur?> AuthenticateAsync(string login, string clearPassword);
    Task<List<Role>> GetRolesAsync();
    Task<int> AddUtilisateurAsync(Utilisateur utilisateur, string clearPassword);
}
