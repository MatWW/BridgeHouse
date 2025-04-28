namespace Backend.Exceptions;

public class RemovePlayerConflictException : Exception
{
    public RemovePlayerConflictException(string message) : base(message) { }
}
