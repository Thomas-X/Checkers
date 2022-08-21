namespace Checkers.Exceptions;

public class InputException : Exception
{
    public string ValidationMessage { get; }

    public InputException(string validationMessage)
    {
        ValidationMessage = validationMessage;
    }
}