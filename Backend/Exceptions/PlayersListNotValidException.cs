namespace Backend.Exceptions;

public class PlayersListNotValidException : Exception
{
    public PlayersListNotValidException(string message) : base(message) { }
}
