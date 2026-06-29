using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace gestionUsuarios.Exception;

public class ExceptionHandler : IExceptionHandler 
{
    private readonly ILogger<ExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    
    public ExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
    {
        //_logger.LogError(exception, "Exception not handle: {Message}", exception.Message);

        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = exception switch
            {
                NotFoundException => CreateProblemDetails(StatusCodes.Status404NotFound, "Resourse not found", exception),
                _ => CreateProblemDetails(StatusCodes.Status500InternalServerError, "Internal server error", exception)
            }
        };

        httpContext.Response.StatusCode = problemDetailsContext.ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(problemDetailsContext);
    }

    private static ProblemDetails CreateProblemDetails(int statusCode, string title, System.Exception exception)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Type = exception.GetType().Name
        };
    }
}