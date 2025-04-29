namespace Backend.Exceptions;

public class IllegalBidException : Exception
{
    public IllegalBidException(string message) : base(message) { }
}
