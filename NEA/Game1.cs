using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Physics;
using System.Collections.Generic;

namespace NEA;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private PhysicsObject _player;
    private List<PhysicsObject> _platforms;
    private List<Rectangle> _worldColliders;
    private CollisionManager _collisionManager;

    private const float MoveSpeed = 200f;
    private const float JumpImpulse = -400f;

    // Virtual resolution: everything is drawn at this size, then scaled to fit the window.
    private const int VirtualWidth = 1600;
    private const int VirtualHeight = 1200; // 4:3

    private RenderTarget2D _renderTarget;
    private Rectangle _destinationRectangle;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        // Start windowed at the virtual resolution; window is resizable so it can
        // expand to fit larger screens.
        _graphics.PreferredBackBufferWidth = VirtualWidth;
        _graphics.PreferredBackBufferHeight = VirtualHeight;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _collisionManager = new CollisionManager();
        _platforms = new List<PhysicsObject>();
        _worldColliders = new List<Rectangle>();

        // Player: dynamic, 32x32
        _player = new PhysicsObject(new Vector2(100, 100), 32, 32);

        // Platforms, positioned in virtual-resolution space (1600x1200)
        AddPlatform(new Vector2(800, 1100), 1400, 50); // ground
        AddPlatform(new Vector2(400, 800), 300, 30);
        AddPlatform(new Vector2(1100, 550), 300, 30);

        base.Initialize();
    }

    private void AddPlatform(Vector2 position, float width, float height)
    {
        PhysicsObject platform = new PhysicsObject(position, width, height)
        {
            Rigidbody = { IsKinematic = true }
        };
        _platforms.Add(platform);
        _worldColliders.Add(platform.Bounds);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _renderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);
        RecalculateDestinationRectangle();
    }

    private void OnResize(object sender, System.EventArgs e)
    {
        RecalculateDestinationRectangle();
    }

    // Computes the largest 4:3 rectangle that fits inside the current window,
    // centered, producing letterbox or pillarbox bars as needed.
    private void RecalculateDestinationRectangle()
    {
        int windowWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int windowHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        float targetAspect = VirtualWidth / (float)VirtualHeight;
        float windowAspect = windowWidth / (float)windowHeight;

        int destWidth, destHeight;

        if (windowAspect > targetAspect)
        {
            // Window is wider than 4:3 -> pillarbox (bars on left/right)
            destHeight = windowHeight;
            destWidth = (int)(windowHeight * targetAspect);
        }
        else
        {
            // Window is taller than 4:3 -> letterbox (bars on top/bottom)
            destWidth = windowWidth;
            destHeight = (int)(windowWidth / targetAspect);
        }

        int x = (windowWidth - destWidth) / 2;
        int y = (windowHeight - destHeight) / 2;

        _destinationRectangle = new Rectangle(x, y, destWidth, destHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        Rigidbody playerRb = _player.Rigidbody;

        if (keyboard.IsKeyDown(Keys.A))
            playerRb.Velocity = new Vector2(-MoveSpeed, playerRb.Velocity.Y);
        else if (keyboard.IsKeyDown(Keys.D))
            playerRb.Velocity = new Vector2(MoveSpeed, playerRb.Velocity.Y);
        else
            playerRb.Velocity = new Vector2(0, playerRb.Velocity.Y);

        if (keyboard.IsKeyDown(Keys.Space) && playerRb.IsGrounded)
            playerRb.AddImpulse(new Vector2(0, JumpImpulse));

        _player.Update(gameTime);
        _collisionManager.ResolveCol(_player, _worldColliders);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // 1. Draw the game scene into the fixed-size render target.
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (var platform in _platforms)
            _spriteBatch.Draw(_pixel, platform.Bounds, Color.Green);

        _spriteBatch.Draw(_pixel, _player.Bounds, Color.Red);

        _spriteBatch.End();

        // 2. Draw the render target to the actual screen, scaled to fit with 4:3 preserved.
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black); // letterbox/pillarbox bar color

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, _destinationRectangle, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
