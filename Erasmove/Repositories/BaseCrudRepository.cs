using Erasmove.Database;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public abstract class BaseCrudRepository<T>
{
    protected readonly DatabaseHelper _dbHelper;

    protected BaseCrudRepository(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    protected abstract string SelectProcedure { get; }
    protected abstract string InsertProcedure { get; }
    protected abstract string UpdateProcedure { get; }
    protected abstract string DeleteProcedure { get; }

    protected abstract T MapItem(SqlDataReader reader);
    protected abstract SqlParameter[] GetInsertParameters(T item);
    protected abstract SqlParameter[] GetUpdateParameters(T item);
    protected abstract SqlParameter[] GetDeleteParameters(int id);

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbHelper.ExecuteReaderAsync(SelectProcedure, MapItem);
    }

    public async Task<bool> AddAsync(T item)
    {
        return await _dbHelper.ExecuteNonQueryAsync(InsertProcedure, GetInsertParameters(item)) > 0;
    }

    public async Task<bool> UpdateAsync(T item)
    {
        return await _dbHelper.ExecuteNonQueryAsync(UpdateProcedure, GetUpdateParameters(item)) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _dbHelper.ExecuteNonQueryAsync(DeleteProcedure, GetDeleteParameters(id)) > 0;
    }
}