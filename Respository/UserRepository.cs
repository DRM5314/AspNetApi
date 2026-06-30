using gestionUsuarios.Enum;
using gestionUsuarios.Exception;
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

    private async Task insertUserRoles(int userId,IEnumerable<RoleType> roles, IDbTransaction transaction )
    {
        if (roles == null || !roles.Any()) return;
        
        const string sqlRoleId = @"SELECT id from role where code = @code";
        
        var usersInsert = new List<Object>();
        foreach (var role in roles)
        {
            int rolId = await _dbConnection.ExecuteScalarAsync<int>(sqlRoleId,
                new { code = role.ToString() },transaction);
            usersInsert.Add(new { userid = userId, rolid = rolId });
        }
        
        const String sqlUserRoles = @"INSERT INTO userroles (userid,rolid) VALUES (@userid, @rolid)";
        
        await _dbConnection.ExecuteAsync(sqlUserRoles,usersInsert,transaction);
        
    }

    public async Task<int> CreateUser(UserCreateRequestDTO user)
    {
        if(_dbConnection.State  != ConnectionState.Open)
            _dbConnection.Open();
        
        var transaction =  _dbConnection.BeginTransaction();
        try
        {
            const String sql = @"INSERT INTO user_ (name, lastName, email)
                           VALUES (@name, @lastName, @email)
                           RETURNING id;";
            int newUserId = await _dbConnection.ExecuteScalarAsync<int>(sql, 
                new { name = user.name, lastName = user.lastName, email = user.email }
                ,transaction);
            
            await insertUserRoles(newUserId, user.roles, transaction);
            
            transaction.Commit();
            return newUserId;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
            transaction.Rollback();
            throw;
        }
    }

    public async Task<int> UpdateUser(int id, Models.User user)
    {
        if (_dbConnection.State  != ConnectionState.Open)
            _dbConnection.Open();
        var transaction = _dbConnection.BeginTransaction();
        try
        {
            const string sqlUserUpdate = @"UPDATE user_ SET name = @name, lastName = @lastName, email = @email WHERE id = @id;";
            int rowsAffected = await _dbConnection.ExecuteAsync(sqlUserUpdate, user);
            if (rowsAffected == 0)
                throw new NotFoundException($"User with id = {id} not found");
            
            const String sqlRemoveRolesUser = @"DELETE FROM userroles WHERE userid = @id;";
            
            await _dbConnection.ExecuteAsync(sqlRemoveRolesUser, new {id = id}, transaction);
            
            await insertUserRoles(id, user.roles, transaction);
            
            transaction.Commit();
            return rowsAffected;
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            transaction.Rollback();
            throw;
        }
    }

    public Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }
}