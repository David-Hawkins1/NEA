
using System;
using System.Collections.Generic;
using Gum;
using Gum.Forms.DefaultVisuals.V3;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.UI;

public class ButtonManager
{
    private readonly List<ImageButton> _buttons = new();

    public void AddButton(
        Panel panel,
        Texture2D normalTexture,
        Texture2D hoverTexture,
        int yPosition,
        Action onClick,
        int width = 300,
        int height = 116)
    {
        Button button = new Button();

        button.Width = width;
        button.Height = height;

        button.Anchor(Gum.Wireframe.Anchor.Center);
        button.X = 0;
        button.Y = yPosition;

        ButtonVisual visual = (ButtonVisual)button.Visual;

        visual.Background.Visible = false;
        visual.TextInstance.Visible = false;

        button.Click += (sender, args) => onClick();

        panel.AddChild(button);

        Rectangle bounds = new Rectangle(
            (Core.VirtualWidth - width) / 2,
            (Core.VirtualHeight / 2) + yPosition - (height / 2),
            width,
            height
        );

        _buttons.Add(new ImageButton(
            button,
            normalTexture,
            hoverTexture,
            bounds
        ));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        MouseState mouse = Mouse.GetState();

        Rectangle viewport = Core.Viewport;

        if (viewport.Width <= 0 || viewport.Height <= 0)
            return;

        Point screenPosition = new Point(mouse.X, mouse.Y);

        Point mousePosition = new Point(
            (mouse.X - viewport.X) * Core.VirtualWidth / viewport.Width,
            (mouse.Y - viewport.Y) * Core.VirtualHeight / viewport.Height
        );

        int hoveredIndex = -1;

        if (viewport.Contains(screenPosition))
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].Contains(mousePosition))
                {
                    hoveredIndex = i;
                    break;
                }
            }
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].Draw(
                spriteBatch,
                i == hoveredIndex
            );
        }
    }

    public void Clear()
    {
        _buttons.Clear();
    }
}
