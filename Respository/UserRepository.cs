using gestionUsuarios.Enum;
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

    private async Task<IEnumerable<User>> dynamicQueryGetUser(string filter = "", object? param = null)
    {
        string sql = $@"SELECT u.id, u.name, u.lastname, u.email, r.code FROM user_ u
                             left join userroles ur on u.id = ur.userid
                             left join role r on ur.rolid = r.id
                             {filter}";
        
        var userDictionary = new Dictionary<int, User>();


        await _dbConnection.QueryAsync<User, RoleRow, User>(
            sql, (user, roleRow) =>
            {
                if (!userDictionary.TryGetValue(user.id, out var userExist))
                {
                    userExist = user;
                    userDictionary.Add(userExist.id, userExist);
                }

                if (roleRow != null)
                {
                    userExist.roles.Add(roleRow.Code);
                }
                return userExist;
            },
            param,
            splitOn:"code"
        );
        return userDictionary.Values;
    }

public async Task<IEnumerable<Models.User>> GetAllUsers()
    {
        /*
         Consult original to obtains users with role (Code not id in postgres)
         select  u.name, u.lastname, r.code from user_ u 
            left join userroles ur on u.id = ur.userid 
            left join role r on ur.rolid = r.id;
         */
        return await dynamicQueryGetUser();
    }

    public async Task<Models.User> GetUserById(int id)
    {
        var result = await dynamicQueryGetUser("where u.id = @id", new { id = id});
        return result.FirstOrDefault();
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