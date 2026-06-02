using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Physics;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;

namespace NEA;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D pixel;
    private Texture2D spriteTexture;
    private Sprite playerSprite;

    private Rigidbody player;
    private Rigidbody ground;
    private List<Rigidbody> platforms;
    private List<Rectangle> worldColliders;

    private CollisionManager collisionManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        collisionManager = new CollisionManager();
        platforms = new List<Rigidbody>();
        worldColliders = new List<Rectangle>();

        // Dynamic player
        player = new Rigidbody(
            new Vector2(100, 100),
            32,
            32
        );
        player.GravityScale = 1f;
        player.UseGravity = true;
        player.Drag = 0.1f;
        player.Friction = 0.3f;

        // Static ground
        ground = new Rigidbody(
            new Vector2(400, 750),
            800,
            50
        );
        ground.IsKinematic = true;
        platforms.Add(ground);
        worldColliders.Add(ground.Bounds);

        // Platform 1
        Rigidbody platform1 = new Rigidbody(
            new Vector2(200, 600),
            200,
            20
        );
        platform1.IsKinematic = true;
        platforms.Add(platform1);
        worldColliders.Add(platform1.Bounds);

        // Platform 2
        Rigidbody platform2 = new Rigidbody(
            new Vector2(600, 500),
            200,
            20
        );
        platform2.IsKinematic = true;
        platforms.Add(platform2);
        worldColliders.Add(platform2.Bounds);

        // Platform 3
        Rigidbody platform3 = new Rigidbody(
            new Vector2(150, 400),
            150,
            20
        );
        platform3.IsKinematic = true;
        platforms.Add(platform3);
        worldColliders.Add(platform3.Bounds);

        // Platform 4
        Rigidbody platform4 = new Rigidbody(
            new Vector2(650, 300),
            150,
            20
        );
        platform4.IsKinematic = true;
        platforms.Add(platform4);
        worldColliders.Add(platform4.Bounds);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        spriteTexture = Content.Load<Texture2D>("images/sprite");
        TextureRegion region = new TextureRegion(spriteTexture, 0, 0, (int)player.Width, (int)player.Height);
        playerSprite = new Sprite(region);
        playerSprite.CenterOrigin();
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
        if (keyboard.IsKeyDown(Keys.Space) && player.IsGrounded)
        {
            player.AddImpulse(new Vector2(0, -400));
        }

        // Stop downward velocity if grounded
        if (player.IsGrounded && player.Velocity.Y > 0)
        {
            player.Velocity = new Vector2(player.Velocity.X, 0);
        }

        // Update physics
        player.Update(gameTime);
        collisionManager.ResolveCol(player, worldColliders);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw platforms
        foreach (var platform in platforms)
        {
            DrawBody(platform, Color.Green);
        }

        // Draw player sprite
        playerSprite.Draw(_spriteBatch, player.Position);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawBody(Rigidbody body, Color color)
    {
        _spriteBatch.Draw(pixel, body.Bounds, color);
    }
}
