using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gum.Forms.Controls;

namespace MonoGameLibrary.UI;

public class ImageButton
{
    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoverTexture;
    private readonly Rectangle _bounds;

    public Button GumButton { get; }

    public ImageButton(
        Button gumButton,
        Texture2D normalTexture,
        Texture2D hoverTexture,
        Rectangle bounds)
    {
        GumButton = gumButton;
        _normalTexture = normalTexture;
        _hoverTexture = hoverTexture;
        _bounds = bounds;
    }

    public void Draw(SpriteBatch spriteBatch, Point mousePosition)
    {
        bool hovered = _bounds.Contains(mousePosition);

        Texture2D texture = hovered
            ? _hoverTexture
            : _normalTexture;

        if (hovered)
        {
            spriteBatch.Draw(texture, _bounds, Color.White);
        }
        else
        {
            float scale = _bounds.Width / (float)_hoverTexture.Width;

            int width = (int)(_normalTexture.Width * scale);
            int height = (int)(_normalTexture.Height * scale);

            Rectangle normalBounds = new Rectangle(
                _bounds.Center.X - width / 2,
                _bounds.Center.Y - height / 2,
                width,
                height
            );

            spriteBatch.Draw(texture, normalBounds, Color.White);
        }
    }
}
