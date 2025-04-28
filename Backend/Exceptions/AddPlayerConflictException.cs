namespace Backend.Exceptions;
public class AddPlayerConflictException : Exception
{
    public AddPlayerConflictException(string message) : base(message) { }
}
