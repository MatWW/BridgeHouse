using Backend.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ExceptionHandlers;

public class PlayerNotFoundInGameExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
       HttpContext httpContext,
       Exception exception,
       CancellationToken cancellationToken)
    {
        if (exception is not PlayerNotFoundInGameException playerNotFoundInGameException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = playerNotFoundInGameException.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
