using Checkers.Exceptions;
using Checkers.Parsers;

var width = 10;
var height = 10;
var colSize = 8;
var game = new Game(width, height, colSize);
var controller = new Controller(game);

while (true)
{
    try
    {
        try
        {
            Console.WindowWidth = 160;
        }
        catch (Exception)
        {
            // ignored
        }

        if (Console.WindowWidth < 144)
        {
            AnsiConsole.Markup("[bold red]Terminal width must be bigger than 143 cols. Please resize your terminal and try again[/]");
            Console.ReadKey();
            break;
        }
        
        
        if (game.HasWinner())
        {
            AnsiConsole.Markup("\n\n\n\n\n[bold green]You win![/]\n\n\n\n\n");
            Console.ReadKey();
            break;
        }
        
        game.Render();
        var selectMove = AnsiConsole.Ask<string>("Select your stone (example: j4) :");
        var selectedCoords = InputParser.Parse(selectMove);
        var move = AnsiConsole.Ask<string>("Select your move (example: i5) :");
        var moveCoords = InputParser.Parse(move);

        controller
            .MoveFrom(selectedCoords.x, selectedCoords.y)
            .To(moveCoords.x, moveCoords.y)
            .Go();
    }
    catch (MoveException e)
    {
        AnsiConsole.Markup($"[bold red]{e.MoveMessage}[/] Please try again in 2s.");
        Thread.Sleep(2000);
    }
    catch (InputException e)
    {
        AnsiConsole.Markup($"[bold red]{e.ValidationMessage}[/] Please try again in 2s.");
        Thread.Sleep(2000);
    }

}