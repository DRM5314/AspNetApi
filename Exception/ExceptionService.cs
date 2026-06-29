namespace gestionUsuarios.Exception;

public class ExceptionService: System.Exception
{
    public int statusCode { get; init; }
    public string title { get; init; }

    public ExceptionService(int statusCode, string title, string message) : base(message)
    {
        this.statusCode = statusCode;
        this.title = title;
    }
}