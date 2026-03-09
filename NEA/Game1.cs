using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Physics;

namespace PlatformerTest;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D pixel;

    private PhysicsWorld physicsWorld;

    private Rigidbody player;
    private Rigidbody ground;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        physicsWorld = new PhysicsWorld();

        // Dynamic player
        player = new Rigidbody(
            new Vector2(100, 100),
            32,
            32
        );

        // Static ground
        ground = new Rigidbody(
            new Vector2(0, 400),
            800,
            50
        );
        ground.IsKinematic = true;

        physicsWorld.AddBody(player);
        physicsWorld.AddBody(ground);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        float speed = 200f;

        // Horizontal movement
        if (keyboard.IsKeyDown(Keys.A))
        {
            player.Velocity = new Vector2(-speed, player.Velocity.Y);
        }
        else if (keyboard.IsKeyDown(Keys.D))
        {
            player.Velocity = new Vector2(speed, player.Velocity.Y);
        }
        else
        {
            player.Velocity = new Vector2(0, player.Velocity.Y);
        }

        // Jump
        if (keyboard.IsKeyDown(Keys.Space))
        {
            player.Velocity = new Vector2(player.Velocity.X, -300);
        }

        physicsWorld.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    private void DrawBody(Rigidbody body, Color color)
    {
        _spriteBatch.Draw(pixel, body.Bounds, color);
    }

}
