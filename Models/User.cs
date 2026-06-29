using gestionUsuarios.Enum;

namespace gestionUsuarios.Models;

public class User
{
    public int id { get; private set; }
    public String name { get; private set; }
    public String lastName { get; private set; }
    public String email { get; private set; }
    
    public HashSet<RoleType> roles { get; private set; } = new HashSet<RoleType>();

    public void setName(string name)
    {
        this.name = name;
    }

    public void setLastName(string lastName)
    {
        this.lastName = lastName;
    }

    public void setEmail(string email)
    {
        this.email = email;
    }
}

public class UserCreateRequestDTO
{
    public string name { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
}

public class UserCreateResponseDTO
{
    public string name { get; }
    public string email { get;}
    public HashSet<RoleType> roles { get; }

    public UserCreateResponseDTO(User user)
    {
        this.name = user.name;
        this.email = user.email;
        this.roles =(user.roles);
    }
}