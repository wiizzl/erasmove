using Erasmove.Database;
using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public class AccountRepository : BaseCrudRepository<Account>
{
    public AccountRepository(DatabaseHelper dbHelper) : base(dbHelper)
    {
    }

    protected override string SelectProcedure => "PSS_ACCOUNT";
    protected override string InsertProcedure => "PSI_ACCOUNT";
    protected override string UpdateProcedure => "PSU_ACCOUNT";
    protected override string DeleteProcedure => "PSD_ACCOUNT";

    protected override Account MapItem(SqlDataReader reader)
    {
        return new Account
        {
            Id = reader.GetInt32(reader.GetOrdinal("ACC_ID")),
            Email = reader.GetString(reader.GetOrdinal("ACC_EMAIL")),
            Password = reader.GetString(reader.GetOrdinal("ACC_PASSWORD")),
            RoleId = reader.GetInt32(reader.GetOrdinal("ACC_ROL_ID"))
        };
    }

    protected override SqlParameter[] GetInsertParameters(Account item) =>
    [
        new("@ACC_EMAIL", item.Email),
        new("@ACC_PASSWORD", item.Password),
        new("@ACC_ROL_ID", item.RoleId)
    ];

    protected override SqlParameter[] GetUpdateParameters(Account item) =>
    [
        new("@ACC_ID", item.Id),
        new("@ACC_EMAIL", item.Email),
        new("@ACC_PASSWORD", item.Password),
        new("@ACC_ROL_ID", item.RoleId)
    ];

    protected override SqlParameter[] GetDeleteParameters(int id) =>
    [
        new("@ACC_ID", id)
    ];
    
    public async Task<Account?> GetAccountByEmailAsync(string email)
    {
        SqlParameter[] parameters = [ new("@ACC_EMAIL", email) ];
        
        var accounts = await _dbHelper.ExecuteReaderAsync("PSS_ACCOUNT_BY_EMAIL", MapItem, parameters);
        
        return accounts.Count > 0 ? accounts[0] : null;
    }
}