using Backend.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            PlayersListNotValidException => (StatusCodes.Status400BadRequest, "Players list not valid"),
            GamePhaseException => (StatusCodes.Status400BadRequest, "Game is in different phase"),
            IllegalBidException => (StatusCodes.Status403Forbidden, "Bid is not allowed"),
            IllegalCardPlayException => (StatusCodes.Status403Forbidden, "Card play is not allowed"),
            UnauthorizedGameActionException => (StatusCodes.Status403Forbidden, "Action not allowed"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Access not allowed"),
            BridgeTableOwnershipException => (StatusCodes.Status403Forbidden, "User is not owner of this table"),
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found"),
            BridgeTableNotFoundException => (StatusCodes.Status404NotFound, "Bridge table not found"),
            GameNotFoundException => (StatusCodes.Status404NotFound, "Game not found"),
            InviteNotFoundException => (StatusCodes.Status404NotFound, "Invite not found"),
            PlayerNotFoundAtBridgeTableException => (StatusCodes.Status404NotFound, "Player not found at bridge table"),
            PlayerNotFoundInGameException => (StatusCodes.Status404NotFound, "Player not found in game"),
            GameAlreadyStartedException => (StatusCodes.Status409Conflict, "Game already started"),
            AddPlayerConflictException => (StatusCodes.Status409Conflict, "Cannot add player"),
            UserAlreadyPartOfTheTableException => (StatusCodes.Status409Conflict, "User already part of the table"),
            PositionAtTableAlreadyTakenException => (StatusCodes.Status409Conflict, "Position at table already taken"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
