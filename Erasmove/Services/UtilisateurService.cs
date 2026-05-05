using System.Data;
using Erasmove.Helpers;
using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class UtilisateurService : BaseCrudService<Utilisateur>, IUtilisateurService
{
    protected override string GetListProcedure => "PSS_UTILISATEUR";
    protected override string DeleteProcedure => "PSD_UTILISATEUR";
    protected override string IdParameterName => "@UTI_ID";

    public UtilisateurService(DatabaseService db) : base(db) { }

    protected override Utilisateur MapEntity(SqlDataReader reader)
    {
        return new Utilisateur
        {
            Id = (int)reader["UTI_ID"],
            Nom = (string)reader["UTI_NOM"],
            Prenom = (string)reader["UTI_PRENOM"],
            Login = (string)reader["UTI_LOGIN"],
            RoleId = (int)reader["ROL_ID"],
            RoleLibelle = (string)reader["ROL_LIBELLE"]
        };
    }

    public async Task<Utilisateur?> AuthenticateAsync(string login, string clearPassword)
    {
        var parameters = new[]
        {
            new SqlParameter("@UTI_LOGIN", login),
            new SqlParameter("@UTI_MOTDEPASSE", SecurityHelper.HashPassword(clearPassword))
        };

        var users = await Db.ExecuteQueryAsync("PSS_UTILISATEUR_LOGIN", MapEntity, parameters);

        return users.FirstOrDefault();
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        return await Db.ExecuteQueryAsync("PSS_ROLE", reader => new Role
        {
            Id = (int)reader["ROL_ID"],
            Libelle = (string)reader["ROL_LIBELLE"]
        });
    }

    public async Task<int> AddUtilisateurAsync(Utilisateur utilisateur, string clearPassword)
    {
        var parameters = new[]
        {
            new SqlParameter("@UTI_NOM", utilisateur.Nom),
            new SqlParameter("@UTI_PRENOM", utilisateur.Prenom),
            new SqlParameter("@UTI_LOGIN", utilisateur.Login),
            new SqlParameter("@UTI_MOTDEPASSE", SecurityHelper.HashPassword(clearPassword)),
            new SqlParameter("@ROL_ID", utilisateur.RoleId),
            new SqlParameter("@NEW_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        return await Db.ExecuteNonQueryAsync("PSI_UTILISATEUR", parameters, "@NEW_ID");
    }

    public async Task UpdateUtilisateurAsync(Utilisateur utilisateur, string? clearPassword = null)
    {
        object passwordParameter = string.IsNullOrWhiteSpace(clearPassword)
            ? DBNull.Value
            : SecurityHelper.HashPassword(clearPassword);

        var parameters = new[]
        {
            new SqlParameter("@UTI_ID", utilisateur.Id),
            new SqlParameter("@UTI_NOM", utilisateur.Nom),
            new SqlParameter("@UTI_PRENOM", utilisateur.Prenom),
            new SqlParameter("@UTI_LOGIN", utilisateur.Login),
            new SqlParameter("@UTI_MOTDEPASSE", passwordParameter),
            new SqlParameter("@ROL_ID", utilisateur.RoleId)
        };

        await Db.ExecuteNonQueryAsync("PSU_UTILISATEUR", parameters);
    }
}