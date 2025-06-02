namespace Backend.Exceptions;

public class UserAlreadyPartOfTheTableException : Exception
{
    public UserAlreadyPartOfTheTableException(string message) : base(message) { }
}
