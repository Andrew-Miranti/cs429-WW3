using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Window : GameWindow
{
    private Game game;
    private int playerID;
    private Army army;
    private int clickFlag = 0; // 0: initial state, 1: army clicked, 2: Confirmation step, clicking on the same spot will decrement it
    private Pos pos;

    public Window(int width, int height, Game game)
        : base(width, height, GraphicsMode.Default, "WW3")
    {
        this.game = game;
        VSync = VSyncMode.On;
    }

    public void Render(Army army)
    {
        var pos = game.Manager.ArmyPosition(army);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.Translate(pos.X, pos.Y, 0);
        GL.Begin(PrimitiveType.Triangles);
        GL.Vertex2(0.7f, 0.3f);
        GL.Vertex2(0.5f, 0.7f);
        GL.Vertex2(0.3f, 0.3f);
        GL.End();
        GL.PopMatrix();
    }

    public void Render(City city)
    {
        Color.GREEN.Use();
        float border = (1.0f - City.SIZE) / 2;
        GL.Rect(border, border, 1.0f - border, 1.0f - border);
    }

    public void Render(Province province)
    {
        Color c = province.Owner?.Color ?? new Color(0.5f, 0.5f, 0.5f);

        c.Use();
        GL.Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (province.City != null)
        {
            Render(province.City);
        }
    }

    public void Render(World world)
    {
        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.Translate(x, y, 0);
                Render(world.GetProvinceAt(new Pos(x, y)));
                GL.PopMatrix();
            }
        }

        GL.Begin(PrimitiveType.Lines);
        Color.BLACK.Use();
        for (int x = 0; x < World.WIDTH; x++)
        {
            GL.Vertex2(x, 0);
            GL.Vertex2(x, World.HEIGHT);
        }

        for (int y = 0; y < World.HEIGHT; y++)
        {
            GL.Vertex2(0, y);
            GL.Vertex2(World.WIDTH, y);
        }

        GL.End();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0.0, World.WIDTH, 0.0, World.HEIGHT, 1.0, -1.0);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // not strictly necessary
        if (Keyboard[Key.Escape])
        {
            Exit();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        World world = game.World;
        Render(world);
        foreach (var player in game.Players)
        {
            player.Color.Use();
            foreach (var army in player.ArmyList)
            {
                Render(army);
            }
        }

        SwapBuffers();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        var a = Mouse.GetCursorState();
        float xratio = (float)Width / (float)World.WIDTH;
        float yratio = (float)Height / (float)World.HEIGHT;
        int x = (int)((float)a.X / (float)xratio);
        int y = World.HEIGHT - (int)((float)a.Y / (float)yratio) - 1; // Subtract the Vasi's bars from the screen height
        Console.WriteLine("x is: " + x + " y is: " + y);
        Player player = game.CurrentPlayer;
        if (clickFlag == 0)
        {
            playerID = game.CurrentPlayerIndex;
            pos = new Pos(x, y);
            army = game.Manager.ArmyAt(pos);
            if (army != null)
            {
                Console.WriteLine("Army clicked.");
                clickFlag = 1;
            }
            else
            {
                Console.WriteLine("Invalid click, not an army. Try again.");
            }
        }
        else if (clickFlag == 1)
        {
            pos = new Pos(x, y);
            if (game.Manager.CanMoveTo(army, pos) == true)
            {
                Console.WriteLine("Press 'y' now to confirm move.");
                clickFlag = 2;
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }
    }

    protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
    {
        if (e.KeyChar == 'y' && clickFlag == 2)
        {
            game.Manager.MoveArmy(army, pos);
            Console.WriteLine("Army has moved.");
            clickFlag = 0;
        }
    }
}