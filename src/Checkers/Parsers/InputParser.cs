using Checkers.Exceptions;

namespace Checkers.Parsers;

public static class InputParser
{
    public static (int y, int x) Parse(string input)
    {
        ValidateInputValue(input);

        // Shamelessly stolen from https://stackoverflow.com/a/20044767
        var numberInAlphabet = char.ToUpper(input[0]) - 64;
        var x = numberInAlphabet - 1; // 0-based index
        var num = input.Substring(1);
        var y = int.Parse(num) - 1; // 0-based index

        return (y, x);
    }

    private static void ValidateInputValue(string input)
    {
        if (input.Length < 2)
        {
            throw new InputException($"Input too short. Input should be (letter)(number). Example: j2.");
        }

        var num = input.Substring(1);
        
        if (!int.TryParse(num, out _))
        {
            throw new InputException($"Input second character should be a number. Example: j2. Actual: {input[1]} ");
        }
    }
}