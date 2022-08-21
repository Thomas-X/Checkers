using Checkers.Exceptions;

namespace Checkers;

public class Controller
{
    public Game Game { get; }

    private int _selectedX;
    private int _selectedY;

    private int _moveX;
    private int _moveY;

    public Controller(Game game)
    {
        Game = game;
    }

    public Controller MoveFrom(int x, int y)
    {
        if (x >= Game.Width || y >= Game.Height)
        {
            throw new InputException("Selected tile is out of bounds.");
        }

        _selectedX = x;
        _selectedY = y;

        return this;
    }

    public Controller To(int x, int y)
    {
        if (x  >= Game.Width || y >= Game.Height)
        {
            throw new InputException("The tile you wanted to move to is out of bounds.");
        }

        _moveX = x;
        _moveY = y;

        return this;
    }

    public void Go()
    {
        var selectedTile = Game.State[_selectedY, _selectedX];
        var moveTile = Game.State[_moveY, _moveX];

        if (moveTile is CheckerState.EmptyLightChecker)
        {
            throw new MoveException("You can't move to a light tile.");
        }

        CheckIfHitIsRequiredAndInputMatchesIt();
        
        CheckIfSelectedTileBelongsToPlayer(selectedTile);
        CheckIfMoveIsBackwards();
        CheckIfMoveIsForward();
        CheckIfMovementSideways();
        CheckIfTileIsAlreadyOccupiedBySamePlayer(selectedTile, moveTile);
        CheckIfMovementIsTooFar();
        CheckIfMovementIsToOwnStone(selectedTile, moveTile);


        SetIsHitRequiredIfNeeded();

        Game.State[_moveY, _moveX] = selectedTile;

        // You can't go on the bright tiles, one less conditional to make!
        Game.State[_selectedY, _selectedX] = CheckerState.EmptyDarkChecker;

        // Succesful turn, propogate that to the gamestate
        Game.DoATurn();
    }

    private void CheckIfHitIsRequiredAndInputMatchesIt()
    {
        if (Game.IsHitRequired)
        {
            if (_moveX != Game.WhereIsHitRequired.x || _moveY != Game.WhereIsHitRequired.y)
            {
                throw new MoveException("A hit is required, input correct values");
            }    
        }
        
    }

    private void SetIsHitRequiredIfNeeded()
    {
        var checkTopLeft = () => TryGetFromState(_moveX - 1, _moveY - 1);
        var checkTopRight = () => TryGetFromState(_moveX + 1, _moveY - 1);
        var checkBottomRight = () => TryGetFromState(_moveX + 1, _moveY + 1);
        var checkBottomLeft = () => TryGetFromState(_moveX - 1, _moveY + 1);

        var checks = new List<(CheckerState state, int x, int y)>()
        {
            checkTopRight(),
            checkTopLeft(),
            checkBottomRight(),
            checkBottomLeft()
        };
        
        var isHitRequired = checks
            .Any(x => IsTileOpposite(x.state));

        if (isHitRequired)
        {
            Game.WhereIsHitRequired = (_moveX, _moveY);
            Game.IsHitRequired = isHitRequired;
        }
        else
        {
            Game.IsHitRequired = false;
            Game.WhereIsHitRequired = (-1, -1);
        }
    }

    private bool IsTileOpposite(CheckerState checkerState)
    {
        if (Game.Turn is Player.White)
        {
            return checkerState == CheckerState.FilledDarkCheckerWithBlack;
        }

        if (Game.Turn is Player.Black)
        {
            return checkerState == CheckerState.FilledDarkCheckerWithWhite;
        }

        return false;
    }

    private (CheckerState state, int x, int y) TryGetFromState(int x, int y)
    {
        try
        {
            return (Game.State[y, x], x, y);
        }
        catch (IndexOutOfRangeException)
        {
            return (CheckerState.None, x, y);
        }
    }


    private void CheckIfMovementIsToOwnStone(CheckerState selectedTile, CheckerState moveTile)
    {
        // Same tile
        if (selectedTile == moveTile)
        {
            throw new MoveException("You can't move to a tile that is already occupied by yourself.");
        }
    }

    private void CheckIfSelectedTileBelongsToPlayer(CheckerState selectedTile)
    {
        if (selectedTile is CheckerState.EmptyDarkChecker or CheckerState.EmptyLightChecker)
        {
            throw new MoveException("You can't move with an empty tile.");
        }

        if (Game.Turn is Player.White)
        {
            if (selectedTile is CheckerState.FilledDarkCheckerWithBlack)
            {
                throw new MoveException("You can't move with a stone that isn't yours.");
            }
        }

        if (Game.Turn is Player.Black)
        {
            if (selectedTile is CheckerState.FilledDarkCheckerWithWhite)
            {
                throw new MoveException("You can't move with a stone that isn't yours.");
            }
        }
    }


    /// <summary>
    /// Validate that the player is not moving more than 1 y axis level
    /// </summary>
    private void CheckIfMovementIsTooFar()
    {
        if (Math.Abs(_selectedY - _moveY) > 1)
        {
            throw new MoveException("You can't move more than one row at a time.");
        }
    }

    private void CheckIfTileIsAlreadyOccupiedBySamePlayer(CheckerState selectedTile, CheckerState moveTile)
    {
        switch (Game.Turn)
        {
            case Player.White when moveTile is CheckerState.FilledDarkCheckerWithWhite:
                throw new MoveException("You can't move to that location, you already occupy it with another stone.");
            case Player.Black when moveTile is CheckerState.FilledDarkCheckerWithBlack:
                throw new MoveException("You can't move to that location, you already occupy it with another stone.");
            default:
                return;
        }
    }

    private void CheckIfMovementSideways()
    {
        if (_moveY == _selectedY)
        {
            throw new MoveException("You can't move sideways on the same axis.");
        }
    }

    private void CheckIfMoveIsForward()
    {
        if (_selectedX == _moveX)
        {
            throw new MoveException("You can't move in a straight line, keep to the dark checkers");
        }
    }

    private void CheckIfMoveIsBackwards()
    {
        if (Game.Turn == Player.Black)
        {
            // Can't go backwards
            if (_selectedY > _moveY)
            {
                throw new MoveException("You can't go backwards.");
            }
        }

        if (Game.Turn == Player.White)
        {
            if (_selectedY < _moveY)
            {
                throw new MoveException("You can't go backwards.");
            }
        }
    }
}