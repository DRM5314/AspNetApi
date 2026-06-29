using gestionUsuarios.Models;
using gestionUsuarios.Respository;
using Microsoft.AspNetCore.Mvc;

namespace gestionUsuarios.Services;

public class UserService
{
    private readonly IUserRepository  _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserCreateResponseDTO>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return users.Select(u => new UserCreateResponseDTO(u));
    }

    public async Task<int> CreateUser(UserCreateRequestDTO userCreateRequest)
    {
        return await _userRepository.CreateUser(userCreateRequest);
    }
    
    
    public async Task <UserCreateResponseDTO> FindById(int id){
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            Console.WriteLine("User dont exist");
        }
        var userResponse = new UserCreateResponseDTO(user);
        return userResponse;
    }

    public async Task<int> UpdateUser(int id, UserCreateRequestDTO userCreateRequest)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
        {
            Console.WriteLine("User dont exist");
        }

        user.setName(userCreateRequest.name);
        user.setLastName(userCreateRequest.lastName);
        user.setEmail(userCreateRequest.email);

        var rowsAffecter = await _userRepository.UpdateUser(id, user);
        return rowsAffecter;
    }
}