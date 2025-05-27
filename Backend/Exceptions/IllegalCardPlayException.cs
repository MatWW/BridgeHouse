namespace Backend.Exceptions;

public class IllegalCardPlayException : Exception
{
    public IllegalCardPlayException(string message) : base(message) { }
}
