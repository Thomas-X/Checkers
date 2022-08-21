namespace Checkers.Exceptions;

public class MoveException : Exception
{
    public string MoveMessage { get; }

    public MoveException(string moveMessage)
    {
        MoveMessage = moveMessage;
    }
}