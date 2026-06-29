namespace gestionUsuarios.Enum;

public enum RoleType
{
    ADMIN = 1,
    USER = 2,
    GUEST = 3
}

public record RoleRow (RoleType Code);