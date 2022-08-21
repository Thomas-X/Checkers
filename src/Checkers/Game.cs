using Checkers.Statics;
using Spectre.Console.Rendering;

namespace Checkers;

public class Game
{
    public Player Turn { get; set; }
    public int Width { get; }
    public int Height { get; }
    public int ColSize { get; }
    public bool IsHitRequired { get; set; } = false;
    public (int x, int y) WhereIsHitRequired { get; set; } = (-1,-1);

    public CheckerState[,] State;

    public Game(int width,
                int height,
                int colSize)
    {
        Width = width;
        Height = height;
        ColSize = colSize;
        Turn = Player.White;

        State = ConstructInitialState(width, height);
    }

    public bool HasWinner()
    {
        var foundWhite = false;
        var foundBlack = false;
        foreach (var checkerState in State)
        {
            if (checkerState == CheckerState.FilledDarkCheckerWithBlack)
            {
                foundBlack = true;
            }

            if (checkerState == CheckerState.FilledDarkCheckerWithWhite)
            {
                foundWhite = true;
            }
        }

        return (!foundWhite && foundBlack) || (foundWhite && !foundBlack);
    }
    
    public void Render()
    {
        AnsiConsole.Clear();

        var grid = new Grid();

        grid.AddColumn(new GridColumn()
        {
            Width = 2,
            Alignment = Justify.Center
        });

        for (var i = 0; i < Height; i++)
        {
            grid.AddColumn(new GridColumn()
            {
                Width = ColSize,
                NoWrap = true,
            });
        }

        // Instructions / Game data
        grid.AddColumn(new GridColumn()
        {
            Width = 48,
            Alignment = Justify.Center
        });

        AddTopRow(grid);

        // Annoying limitation with the library, you need to first add all the columns before
        // adding the total amount of rows.
        // We could render from top-to-bottom but I prefer left-to-right rendering
        for (var y = 0; y < Height; y++)
        {
            var rows = new List<IRenderable>()
            {
                new Text($"{y + 1}")
            };
            for (var x = 0; x < Width; x++)
            {
                var image = ConvertCheckerStateToImage(State[y, x]);
                rows.Add(image);
            }

            // Somewhere in the middle render the game data and instructions
            if (Height / 2 == y)
            {
                RenderInstructionsAndGameData(rows);
            }

            grid.AddRow(rows.ToArray());
        }

        AnsiConsole.Write(grid);
    }

    public void DoATurn()
    {
        Turn = Turn == Player.White
            ? Player.Black
            : Player.White;
    }

    private void RenderInstructionsAndGameData(List<IRenderable> rows)
    {
        var turnStr = "";
        if (Turn is Player.White)
        {
            turnStr = "[bold white]It is white's turn.[/]";
        }
        else if (Turn is Player.Black)
        {
            turnStr = "[bold yellow]It is black's turn.[/]";
        }

        var hitStr = "";
        if (IsHitRequired)
        {
            hitStr = "\n [bold purple]A hit is required![/]";
        }
        

        rows.Add(
            new Markup($"[bold green]Welcome to RETRO Checkers![/] \n {turnStr} {hitStr}")
        );
    }

    private void AddTopRow(Grid grid)
    {
        List<IRenderable> topRow = new List<IRenderable>()
        {
            new Text("")
        };

        for (int x = 0; x < Width; x++)
        {
            char letter;
            if (x == 0)
            {
                letter = 'a';
            }
            else
            {
                letter = (char)('a' + x);
            }

            topRow.Add(new Text(letter.ToString()));
        }

        grid.AddRow(
            topRow.ToArray()
        );
    }

    private CanvasImage ConvertCheckerStateToImage(CheckerState state) => state switch
    {
        CheckerState.EmptyLightChecker => Assets.EmptyLightChecker,
        CheckerState.EmptyDarkChecker => Assets.EmptyDarkChecker,
        CheckerState.FilledLightCheckerWithWhite => Assets.FilledLightCheckerWithWhite,
        CheckerState.FilledLightCheckerWithBlack => Assets.FilledLightCheckerWithBlack,
        CheckerState.FilledDarkCheckerWithWhite => Assets.FilledDarkCheckerWithWhite,
        CheckerState.FilledDarkCheckerWithBlack => Assets.FilledDarkCheckerWithBlack,
        _ => throw new ArgumentOutOfRangeException()
    };


    private CheckerState[,] ConstructInitialState(int width, int height)
    {
        if (width < 10)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width should at minimum be 10");
        }

        if (height < 10)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height should at minimum be 10");
        }

        var state = new CheckerState[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                // If we're on an even y
                if (y % 2 == 0)
                {
                    // Then check if the x is also even, render light if it is
                    var isXEven = x % 2 == 0;

                    // Fill with black's stones
                    if (y < 4)
                    {
                        state[y, x] = isXEven
                            ? CheckerState.EmptyLightChecker
                            : CheckerState.FilledDarkCheckerWithBlack;
                        continue;
                    }

                    // Fill with white's stones
                    if (y > Height - 5)
                    {
                        state[y, x] = isXEven
                            ? CheckerState.EmptyLightChecker
                            : CheckerState.FilledDarkCheckerWithWhite;
                        continue;
                    }

                    // Fill up the blanks in between
                    state[y, x] = isXEven
                        ? CheckerState.EmptyLightChecker
                        : CheckerState.EmptyDarkChecker;
                }
                // If we're on an uneven y
                else
                {
                    // Then check if the x is also even, render dark if it is
                    var isXEven = x % 2 == 0;

                    // Fill with black's stones
                    if (y < 4)
                    {
                        state[y, x] = isXEven
                            ? CheckerState.FilledDarkCheckerWithBlack
                            : CheckerState.EmptyLightChecker;
                        continue;
                    }

                    // Fill with white's stones
                    if (y > Height - 5)
                    {
                        state[y, x] = isXEven
                            ? CheckerState.FilledDarkCheckerWithWhite
                            : CheckerState.EmptyLightChecker;
                        continue;
                    }

                    state[y, x] = x % 2 == 0
                        ? CheckerState.EmptyDarkChecker
                        : CheckerState.EmptyLightChecker;
                }
            }
        }

        return state;
    }
}