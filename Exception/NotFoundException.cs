namespace gestionUsuarios.Exception;

public class NotFoundException : ExceptionService
{
    public int statusCode { get; }

    public NotFoundException(string message) : base(StatusCodes.Status404NotFound,"Resourse not found",message) {}
}