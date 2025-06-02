namespace Backend.Exceptions;

public class PlayerNotFoundInGameException : Exception
{
    public PlayerNotFoundInGameException(string message) : base(message) { }
}
