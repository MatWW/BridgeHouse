namespace Backend.Exceptions;

public class PlayerNotFoundAtBridgeTableException : Exception
{
    public PlayerNotFoundAtBridgeTableException(string message) : base(message) { }
}
