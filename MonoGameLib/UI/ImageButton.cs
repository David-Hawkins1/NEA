
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameLibrary.UI;

public class ImageButton
{
    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoverTexture;
    private readonly Rectangle _normalSource;
    private readonly Rectangle _hoverSource;
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

        _normalSource = GetVisibleBounds(normalTexture);
        _hoverSource = GetVisibleBounds(hoverTexture);
    }

    private static Rectangle GetVisibleBounds(Texture2D texture)
    {
        Color[] pixels = new Color[
            texture.Width * texture.Height
        ];

        texture.GetData(pixels);

        int minX = texture.Width;
        int minY = texture.Height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width; x++)
            {
                if (pixels[y * texture.Width + x].A <= 8)
                    continue;

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
            }
        }

        if (maxX < minX || maxY < minY)
            return new Rectangle(0, 0, texture.Width, texture.Height);

        return new Rectangle(
            minX,
            minY,
            maxX - minX + 1,
            maxY - minY + 1
        );
    }

    public bool Contains(Point mousePosition)
    {
        return _bounds.Contains(mousePosition);
    }

    public void Draw(SpriteBatch spriteBatch, bool hovered)
    {
        Texture2D texture = hovered
            ? _hoverTexture
            : _normalTexture;

        Rectangle source = hovered
            ? _hoverSource
            : _normalSource;

        // The normal text is deliberately smaller than the hover frame.
        float maxWidth = hovered
            ? _bounds.Width
            : _bounds.Width * 0.75f;

        float maxHeight = hovered
            ? _bounds.Height
            : _bounds.Height * 0.65f;

        float scale = Math.Min(
            maxWidth / source.Width,
            maxHeight / source.Height
        );

        int width = (int)(source.Width * scale);
        int height = (int)(source.Height * scale);

        Rectangle destination = new Rectangle(
            _bounds.Center.X - width / 2,
            _bounds.Center.Y - height / 2,
            width,
            height
        );

        spriteBatch.Draw(
            texture,
            destination,
            source,
            Color.White
        );
    }
}
