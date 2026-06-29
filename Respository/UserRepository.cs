using gestionUsuarios.Models;

namespace gestionUsuarios.Respository;
using System.Data;
using Dapper;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;

    public UserRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<Models.User>> GetAllUsers()
    {
        const string sql = "SELECT * FROM user_";
        return await _dbConnection.QueryAsync<Models.User>(sql);
    }

    public async Task<Models.User> GetUserById(int id)
    {
        const string sql = @"SELECT * FROM user_ WHERE id = @id";
        return await _dbConnection.QueryFirstOrDefaultAsync<Models.User>(sql, new { id });
    }

    public async Task<int> CreateUser(UserCreateRequestDTO user)
    {
        const String sql = @"INSERT INTO user_ (name, lastName, email)
                           VALUES (@name, @lastName, @email)
                           ;";
        return  await _dbConnection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<int> UpdateUser(int id, Models.User user)
    {
        const String sql = @"UPDATE user_ SET name = @name, lastName = @lastName, email = @email WHERE id = @id;";
        int rowsAffected = await _dbConnection.ExecuteAsync(sql, user);
        return rowsAffected;
    }

    public Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }
}