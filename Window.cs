using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Window : GameWindow
{
    public const int HUDPIXELHEIGHT = 100;
    public static readonly Color BACKGROUND = new Color(0.5f, 0.5f, 0.5f);
    public static readonly Color HUDBACKGROUND = new Color(0.09375f, 0.27f, 0.4f);
    public static readonly int BORDER = 2;
    private Game game;
    private int playerID;
    private Army army;
    private int clickFlag = 0; // 0: initial state, 1: army clicked, 2: Confirmation step, clicking on the same spot will decrement it
    private Pos pos;
    private float centerX;
    private float centerY;
    private float scale; // scale = pixels per world square

    private int endTurnButtonX;
    private int endTurnButtonY;
    private int endTurnButtonWidth;
    private int endTurnButtonHeight;

    public Window(int width, int height, Game game)
        : base(width, height, GraphicsMode.Default, "WW3")
    {
        this.game = game;
        scale = 15;
        centerX = World.WIDTH / 2;
        centerY = World.HEIGHT / 2;
        VSync = VSyncMode.On;
        endTurnButtonY = 30;
        endTurnButtonHeight = 45;
        endTurnButtonWidth = 150;
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
        // render provinces
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

        // render gridlines
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

        // render selected province
        GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
        GL.Color3(1.0, 0.5, 0.0);
        GL.Rect(pos.X, pos.Y, pos.X + 1, pos.Y + 1);
        GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
    }

    public void RenderHealth()
    {
        if (army != null)
        {
            var x = 30;
            var y = 30;
            var size = 45;
            var thickness = 10;
            Color.RED.Use();
            GL.Rect(x + (size / 2) - (thickness / 2), y, x + (size / 2) + (thickness / 2), y + size);
            GL.Rect(x, y + (size / 2) - (thickness / 2), x + size, y + (size / 2) + (thickness / 2));
            Color.BLACK.Use();

            var barX = 100;
            var barLength = 100;
            var barFilled = army.Health;
            GL.Rect(barX - BORDER, y - BORDER, barX + barLength + BORDER, y + size + BORDER);
            HUDBACKGROUND.Use();
            GL.Rect(barX, y, barX + barLength, y + size);
            Color.GREEN.Use();
            GL.Rect(barX, y, barX + barFilled, y + size);
        }
    }

    public void RenderEndTurn()
    {
        endTurnButtonX = (Width / 2) - 100;
        Color.WHITE.Use();
        GL.Rect(endTurnButtonX, endTurnButtonY, endTurnButtonX + endTurnButtonWidth, endTurnButtonY + endTurnButtonHeight);
    }

    public void RenderHUD(World world)
    {
        // render HUD background
        GL.Viewport(0, 0, Width, HUDPIXELHEIGHT);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0.0, Width, 0.0, HUDPIXELHEIGHT, 1, -1);
        HUDBACKGROUND.Use();
        GL.Rect(0, 0, Width, HUDPIXELHEIGHT);

        RenderHealth();
        RenderEndTurn();

        // render province info
        Province province = world.GetProvinceAt(pos);

        int passiveFood = province.PassiveResources.GetAmountOf(ResourceType.Food);
        int passiveWeapons = province.PassiveResources.GetAmountOf(ResourceType.Weapons);
        int activeFood = province.ActiveResources.GetAmountOf(ResourceType.Food);
        int activeWeapons = province.ActiveResources.GetAmountOf(ResourceType.Weapons);

        int food = passiveFood;
        int weapons = passiveWeapons;
        var right = Width;
        var foodX = right - 100;
        var gunX = right - 200;
        float squareSize = 40;
        var squareY = (HUDPIXELHEIGHT / 2) - (squareSize / 2);
        int foodSideNum = (int)Math.Sqrt(food) + 1;
        int weaponsSideNum = (int)Math.Sqrt(weapons) + 1;
        float foodSize = squareSize / foodSideNum;
        float weaponSize = squareSize / weaponsSideNum;

        // food
        Color.BLACK.Use();
        GL.Rect(foodX - BORDER, squareY - BORDER, foodX + squareSize + BORDER, squareY + squareSize + BORDER);
        HUDBACKGROUND.Use();
        GL.Rect(foodX, squareY, foodX + squareSize, squareY + squareSize);

        Color.RED.Use();
        for (int i = 0; i < food; i++)
        {
            var x2 = foodX + ((i % foodSideNum) * foodSize);
            var y2 = squareY + ((i / foodSideNum) * foodSize);
            GL.Rect(x2 + 1, y2 + 1, x2 + foodSize - 1, y2 + foodSize - 1);
        }

        // weapons
        Color.BLACK.Use();
        GL.Rect(gunX - BORDER, squareY - BORDER, gunX + squareSize + BORDER, squareY + squareSize + BORDER);
        HUDBACKGROUND.Use();
        GL.Rect(gunX, squareY, gunX + squareSize, squareY + squareSize);
        new Color(0.8f, 0.6f, 0.0f).Use();
        for (int i = 0; i < weapons; i++)
        {
            var x2 = gunX + ((i % weaponsSideNum) * weaponSize);
            var y2 = squareY + ((i / weaponsSideNum) * weaponSize);
            GL.Rect(x2 + 1, y2 + 1, x2 + weaponSize - 1, y2 + weaponSize - 1);
        }
    }

    public float GetLeft()
    {
        return centerX - (Width / 2 / scale);
    }

    public float GetRight()
    {
        return centerX + (Width / 2 / scale);
    }

    public float GetBottom()
    {
        return centerY - ((Height - HUDPIXELHEIGHT) / 2 / scale);
    }

    public float GetTop()
    {
        return centerY + ((Height - HUDPIXELHEIGHT) / 2 / scale);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
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

        var left = GetLeft();
        var right = GetRight();
        var bottom = GetBottom();
        var top = GetTop();

        GL.Viewport(0, HUDPIXELHEIGHT, Width, Height - HUDPIXELHEIGHT);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(left, right, bottom, top, 1.0, -1.0);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        World world = game.GameWorld;
        Render(world);
        foreach (var player in game.Players)
        {
            player.Color.Use();
            foreach (var army in player.ArmyList)
            {
                // if an army's health is 0, they are dead. Remove the army and don't render
                if (army.Health == 0)
                {
                    player.RemoveArmy(army);
                }

                Render(army);
            }

            // RenderResources(player.ResourcesString())
        }

        // render HUD
        RenderHUD(world);

        SwapBuffers();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        var a = Mouse.GetCursorState();
        int mouseX = e.Position.X;
        int mouseY = Height - e.Position.Y - 1;

        float left = GetLeft();
        float right = GetRight();
        float bottom = GetBottom();
        float top = GetTop();

        float worldX = left + (((float)mouseX / Width) * (right - left));
        float worldY = bottom + (((float)(mouseY - HUDPIXELHEIGHT) / (Height - HUDPIXELHEIGHT)) * (top - bottom));

        int x = (int)worldX;
        int y = (int)worldY;
        Console.WriteLine("x is: " + x + " y is: " + y);
        Player player = game.CurrentPlayer;

        // check if the end turn button was clicked
        if (endTurnButtonX <= mouseX && mouseX <= (endTurnButtonX + endTurnButtonWidth) && endTurnButtonY <= mouseY && mouseY <= (endTurnButtonY + endTurnButtonHeight))
        {
            Console.WriteLine("Turn Ended");
            game.EndTurn();
        }

        if (clickFlag == 0 && x >= 0 && x < World.WIDTH && y >= 0 && y < World.HEIGHT)
        {
            playerID = game.CurrentPlayerIndex;
            pos = new Pos(x, y);
            army = game.Manager.ArmyAt(pos);
            if (army != null && player.ArmyList.Contains(army))
            {
                Console.WriteLine("Army clicked.");
                clickFlag = 1;
            }
            else
            {
                Console.WriteLine("Invalid click, not an army. Try again.");
            }
        }
        else if ((clickFlag == 1 || clickFlag == 2) && x >= 0 && x < World.WIDTH && y >= 0 && y < World.HEIGHT)
        {
            pos = new Pos(x, y);
            if (game.Manager.ArmyPosition(army).Equals(pos))
            {
                Console.WriteLine("Army unselected");
                clickFlag = 0;
            }
            else if (game.Manager.CanMoveTo(army, pos) == true)
            {
                Console.WriteLine("Press 'y' now to confirm move.");
                Console.WriteLine("Press 'u' to undo the move");
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

        if (e.KeyChar == 'u' && clickFlag == 2)
        {
            Console.WriteLine("Army has been moved back to its previous position");
            clickFlag = 0;
        }

        if (e.KeyChar == '+')
        {
            scale *= 1.1f;
        }

        if (e.KeyChar == '-')
        {
            scale /= 1.1f;
        }

        if (e.KeyChar == 'w')
        {
            centerY += 1;
        }

        if (e.KeyChar == 'a')
        {
            centerX -= 1;
        }

        if (e.KeyChar == 'd')
        {
            centerX += 1;
        }

        if (e.KeyChar == 's')
        {
            centerY -= 1;
        }

        if (e.KeyChar == 'n')
        {
            game.EndTurn();
            Console.WriteLine("Ended turn");
        }
    }
}
