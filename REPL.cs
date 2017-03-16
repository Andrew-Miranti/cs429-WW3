using System;
using System.Text.RegularExpressions;

public class REPL
{
    private const string Commands = @"help|end|mv|quit|print|capture|feed|resources|undo";
    private static readonly Regex Command = new Regex(@"(" + Commands + @")");
    private static readonly Regex Move = new Regex(@"mv (\d+) (\d+),(\d+)");
    private static readonly Regex Capture = new Regex(@"capture (\d+)");
    private static readonly Regex Feed = new Regex(@"feed (\d+) (\d+)");
    private static readonly Regex Undo = new Regex(@"undo (\d+)");
    private readonly Game game;
    private readonly Window window;

    public REPL(Game game, Window window)
    {
        this.game = game;
        this.window = window;
    }

    public void HelpCommand()
    {
        Console.WriteLine("Help => Prints this help message");
        Console.WriteLine("End => Ends the current player's turn, advancing to the next player");
        Console.WriteLine("mv ArmyId TargetX,TargetY => Moves the army ArmyId owned by the player to TargetX,TargetY iff the move is legal");
        Console.WriteLine("capture ArmyId Changes territory under armyId's control to the current player");
        Console.WriteLine("print => Prints the current state of the world to the terminal");
        Console.WriteLine("feed id => Feeds an army from the player's food stockpile to heal it");
        Console.WriteLine("undo id => Moves an army back to its original position for the turn");
        Console.WriteLine("quit => exits the REPL and closes out the game");
    }

    public void EndCommand()
    {
        game.CurrentPlayer.CommitMoves();
        game.AdvancePlayer();
    }

    public void MoveCommand(string input)
    {
        Player player = game.CurrentPlayer;
        var mv = Move.Match(input);
        if (mv.Success)
        {
            var index = int.Parse(mv.Groups[1].Value);

            var x = int.Parse(mv.Groups[2].Value);
            var y = int.Parse(mv.Groups[3].Value);
            var target = new Pos(x, y);

            Console.WriteLine(" " + index + " " + x + " " + y);

            if (!player.ArmyExists(index))
            {
                Console.WriteLine("Illegal army id");
                return;
            }

            if (player.CanMoveArmy(index, target))
            {
                player.MoveArmy(index, target);
            }
            else
            {
                Console.WriteLine("Movement out of range");
            }
        }
        else
        {
            Console.WriteLine("Command must match: mv [0-9]+ [0-9]+,[0-9]+");
        }
    }

    public void CaptureCommand(string input)
    {
        Player player = game.CurrentPlayer;
        var capture = Capture.Match(input);
        if (capture.Success)
        {
            var index = int.Parse(capture.Groups[1].Value);

            if (player.ArmyExists(index))
            {
                var armyPosition = player.ArmyPosition(index);
                var armyProvince = game.World.GetProvinceAt(armyPosition);
                if (armyProvince.Owner != player)
                {
                    armyProvince.Owner = player;
                    Console.WriteLine("Territory captured");
                }
                else
                {
                    Console.WriteLine("Territory already controlled");
                }
            }
            else
            {
                Console.WriteLine("Invalid Army Index");
            }
        }
        else
        {
            Console.WriteLine("Command must match: capture [0-9]+");
        }
    }

    public void UndoCommand(string input)
    {
        Player player = game.CurrentPlayer;
        var capture = Undo.Match(input);
        if (capture.Success)
        {
            var index = int.Parse(capture.Groups[1].Value);

            if (player.ArmyExists(index))
            {
                player.UndoMove(index);
            }
            else
            {
                Console.WriteLine("Invalid Army Index");
            }
        }
        else
        {
            Console.WriteLine("Command must match: undo [0-9]+");
        }
    }

    public void QuitCommand()
    {
        try
        {
            window.Exit();
        }
        catch (System.ObjectDisposedException)
        {
            Console.WriteLine("Window already closed externally");
        }
    }

    public void PrintCommand()
    {
        game.Print();
    }

    public void ResourcesCommand()
    {
        Console.WriteLine(game.CurrentPlayer.ResourcesString());
    }

    public void FeedCommand(string input)
    {
        Player player = game.CurrentPlayer;
        var feed = Feed.Match(input);
        if (feed.Success)
        {
            var index = int.Parse(feed.Groups[1].Value);
            var foodQuantity = int.Parse(feed.Groups[2].Value);

            if (player.FeedArmy(index, foodQuantity))
            {
                Console.WriteLine("Army fed");
            }
            else
            {
                Console.WriteLine("Army index invalid or not enough food");
            }
        }
    }

    public void Launch()
    {
        Console.WriteLine("Welcome to WW3 - type 'help' for instructions.");

        bool running = true;
        do
        {
            Console.Write(game.CurrentPlayerIndex + "> ");

            var input = Console.ReadLine();
            var match = Command.Match(input);
            if (match.Success)
            {
                if (match.Value == "help")
                {
                    HelpCommand();
                }
                else if (match.Value == "end")
                {
                    EndCommand();
                }
                else if (match.Value == "mv")
                {
                    MoveCommand(input);
                }
                else if (match.Value == "capture")
                {
                    CaptureCommand(input);
                }
                else if (match.Value == "quit")
                {
                    running = false;
                    QuitCommand();
                }
                else if (match.Value == "print")
                {
                    PrintCommand();
                }
                else if (match.Value == "resources")
                {
                    ResourcesCommand();
                }
                else if (match.Value == "feed")
                {
                    FeedCommand(input);
                }
                else if (match.Value == "undo")
                {
                    UndoCommand(input);
                }
            }
            else
            {
                Console.WriteLine("Command must match: " + Commands);
            }
        }
        while (running);
    }
}