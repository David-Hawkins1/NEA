using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Physics;
using MonoGameLibrary.Scenes;
using NEA.Controllers;

namespace NEA.Scenes;

public class GameScene : Scene
{
    private PhysicsWorld _physicsWorld;
    private PhysicsObject _player;
    private List<PhysicsObject> _platforms;
    private PlayerController _playerController;

    public override void Initialize()
    {
        // Calls LoadContent(). Keep this call.
        base.Initialize();

        _physicsWorld = new PhysicsWorld();
        _platforms = new List<PhysicsObject>();

        _player = new PhysicsObject(
            new Vector2(100, 100),
            32,
            32
        );
        _playerController = new PlayerController(_player, Core.Input);
        _physicsWorld.Add(_player);

        AddPlatform(new Vector2(800, 1100), 1400, 50);
        AddPlatform(new Vector2(400, 950), 300, 30);
        AddPlatform(new Vector2(800, 800), 300, 30);
        AddPlatform(new Vector2(1200, 650), 300, 30);
        AddPlatform(new Vector2(0, 200), 100, 2000);
    }

    private void AddPlatform(
        Vector2 position,
        float width,
        float height)
    {
        PhysicsObject platform =
            new PhysicsObject(position, width, height)
            {
                Rigidbody =
                {
                    IsKinematic = true
                }
            };

        _platforms.Add(platform);
        _physicsWorld.AddCollider(platform.Bounds);
    }

    public override void Update(GameTime gameTime)
    {
        // Core has already updated Core.Input this frame.
        _playerController.Update(gameTime);
        _physicsWorld.Step(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // Core has already set the render target and cleared it.
        Core.SpriteBatch.Begin();

        foreach (var platform in _platforms)
        {
            Core.SpriteBatch.Draw(
                Core.Pixel,
                platform.Bounds,
                Color.Green
            );
        }

        Core.SpriteBatch.Draw(
            Core.Pixel,
            _player.Bounds,
            Color.Red
        );

        Core.SpriteBatch.End();

        base.Draw(gameTime);
    }
}
