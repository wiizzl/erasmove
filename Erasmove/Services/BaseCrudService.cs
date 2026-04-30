using Erasmove.Models.Interfaces;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public abstract class BaseCrudService<T> : ICrudService<T> where T : class, IEntity
{
    protected readonly DatabaseService Db;

    protected abstract string GetListProcedure { get; }
    protected abstract string DeleteProcedure { get; }
    protected abstract string IdParameterName { get; }

    protected BaseCrudService(DatabaseService db)
    {
        Db = db;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await Db.ExecuteQueryAsync(GetListProcedure, MapEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var parameters = new[] { new SqlParameter(IdParameterName, id) };
        await Db.ExecuteNonQueryAsync(DeleteProcedure, parameters);
    }

    protected abstract T MapEntity(SqlDataReader reader);
}