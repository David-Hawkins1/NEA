using Microsoft.Xna.Framework;

namespace MonoGameLibrary.UI;

public class ButtonStyle
{
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }
    public Color FocusColor { get; set; }

    public int Width { get; set; } = 280;
    public int Height { get; set; } = 65;

    public ButtonStyle(
        Color backgroundColor,
        Color textColor,
        Color focusColor)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        FocusColor = focusColor;
    }
}
