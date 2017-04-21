using System;
using System.Threading;
using OpenTK;

public class GameRunner
{
    private const string START = "start.json";

    public static void Main(string[] args)
    {
        Game game = null;
        if (args.Length < 1)
        {
            Console.WriteLine("Loading Start");
            game = Game.LoadFromFile(START);
        }
        else
        {
            Console.WriteLine("Loading " + args[0]);
            game = Game.LoadFromFile(args[0]);
        }

        var window = new Window(600, 600, game);
        var repl = new REPL(game, window);
        var task = new Thread(() =>
        {
            repl.Launch();
        });

        task.Start();
        using (GameWindow w = window) // necessary idiom for IDisposable
        {
            w.Run(60.0); // hand off execution to OpenTK, 60fps
        }

        task.Join(); // will execute after window.Exit() function is called
    }
}
