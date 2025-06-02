namespace Backend.Exceptions;

public class PositionAtTableAlreadyTakenException : Exception
{
    public PositionAtTableAlreadyTakenException(string message) : base(message) { }
}
