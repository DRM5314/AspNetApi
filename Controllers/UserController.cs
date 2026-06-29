using gestionUsuarios.Models;
using gestionUsuarios.Services;
using Microsoft.AspNetCore.Mvc;

namespace gestionUsuarios.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]

public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.FindById(id);
        return Ok(user);
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequestDTO dto)
    {
        if (dto == null)
        {
            return BadRequest("Los datos no deben ser vacios");
        }

        var userCreated = await _userService.CreateUser(dto);
        return CreatedAtAction(nameof(CreateUser), new { userCreated }, userCreated);
    }

    [HttpPut (("{id}"))]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateRequestDTO dto)
    {
        await _userService.UpdateUser(id, dto);
        return NoContent();
    }
}