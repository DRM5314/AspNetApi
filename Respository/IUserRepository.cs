using gestionUsuarios.Models;

namespace gestionUsuarios.Respository;

public interface IUserRepository
{
    Task<IEnumerable<Models.User>> GetAllUsers();
    Task<Models.User> GetUserById(int id);
    Task<int> CreateUser(UserCreateRequestDTO user);
    Task<int> UpdateUser(int id, Models.User user);
    Task<bool> DeleteUser(int id);
}