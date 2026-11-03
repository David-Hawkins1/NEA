using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace NEA.Scenes;

public class StartScene : Scene
{
    private const string TitleText = "The Trapped Lily";
    private const string PromptText = "Press Enter to start";

    private SpriteFont _titleFont;
    private SpriteFont _menuFont;

    public override void LoadContent()
    {
        _titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
        _menuFont = Content.Load<SpriteFont>("Fonts/MenuFont");
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.SpriteBatch.Begin();

        DrawCentred(_titleFont, TitleText, Core.VirtualHeight * 0.35f, Color.White);
        DrawCentred(_menuFont, PromptText, Core.VirtualHeight * 0.6f, Color.White);

        Core.SpriteBatch.End();

        base.Draw(gameTime);
    }

    private static void DrawCentred(SpriteFont font, string text, float centreY, Color color)
    {
        Vector2 size = font.MeasureString(text);

        Vector2 position = new Vector2(
            (Core.VirtualWidth - size.X) / 2f,
            centreY - size.Y / 2f);

        Core.SpriteBatch.DrawString(font, text, position, color);
    }
}
