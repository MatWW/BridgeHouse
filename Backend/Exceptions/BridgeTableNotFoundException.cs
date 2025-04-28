namespace Backend.Exceptions;
public class BridgeTableNotFoundException : Exception
{
    public BridgeTableNotFoundException(string message) : base(message) { }
}
