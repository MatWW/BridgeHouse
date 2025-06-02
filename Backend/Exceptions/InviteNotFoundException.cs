namespace Backend.Exceptions;

public class InviteNotFoundException : Exception
{
    public InviteNotFoundException(string message) : base(message) { }
}
