using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace NEA.Scenes;

public class StartScene : Scene
{
    private Texture2D _titleBackground;

    public override void LoadContent()
    {
        _titleBackground =
            Content.Load<Texture2D>("Images/TitleScreen");
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }
        else if (Core.Input.Keyboard.WasKeyJustPressed(Keys.F2))
        {
            Core.ChangeScene(new TilemapTestScene());
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();

        Core.SpriteBatch.Draw(
            _titleBackground,
            new Rectangle(
                0,
                0,
                Core.VirtualWidth,
                Core.VirtualHeight
            ),
            Color.White
        );

        Core.SpriteBatch.End();

        base.Draw(gameTime);
    }
}
