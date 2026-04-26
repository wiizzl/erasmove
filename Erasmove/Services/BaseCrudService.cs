using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public interface ICrudService<T> where T : IEntity
{
    Task<List<T>> GetAllAsync();
    Task DeleteAsync(int id);
}

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