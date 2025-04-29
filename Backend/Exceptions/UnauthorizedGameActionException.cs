namespace Backend.Exceptions;

public class UnauthorizedGameActionException : Exception
{
    public UnauthorizedGameActionException(string message) : base(message) { }
}
