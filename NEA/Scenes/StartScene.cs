using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.UI;
using MonoGameGum;
using Gum.Forms.Controls;

namespace NEA.Scenes;

public class StartScene : Scene
{
    private Texture2D _titleBackground;

    private Texture2D _startNormal;
    private Texture2D _startHover;

    private Texture2D _settingsNormal;
    private Texture2D _settingsHover;

    private Texture2D _exitNormal;
    private Texture2D _exitHover;

    private ButtonManager _buttonManager;

    public override void LoadContent()
    {
        _titleBackground =
            Content.Load<Texture2D>("Images/TitleScreen");

        _startNormal =
            Content.Load<Texture2D>("Images/Buttons/StartGame_Normal");

        _startHover =
            Content.Load<Texture2D>("Images/Buttons/StartGame_Hover");

        _settingsNormal =
            Content.Load<Texture2D>("Images/Buttons/Settings_Normal");

        _settingsHover =
            Content.Load<Texture2D>("Images/Buttons/Settings_Hover");

        _exitNormal =
            Content.Load<Texture2D>("Images/Buttons/Exit_Normal");

        _exitHover =
            Content.Load<Texture2D>("Images/Buttons/Exit_Hover");

        GumService.Default.Root.Children.Clear();

        Panel menuPanel = new Panel();
        menuPanel.Dock(Gum.Wireframe.Dock.Fill);
        menuPanel.AddToRoot();

        _buttonManager = new ButtonManager();

        // Start Game
        _buttonManager.AddButton(
            menuPanel,
            _startNormal,
            _startHover,
            160,
            () => Core.ChangeScene(new GameScene()),
            width: 300
        );

        // Settings
        _buttonManager.AddButton(
            menuPanel,
            _settingsNormal,
            _settingsHover,
            300,
            () => { },
            width: 300
        );

        // Exit
        _buttonManager.AddButton(
            menuPanel,
            _exitNormal,
            _exitHover,
            440,
            () => Core.Instance.Exit(),
            width: 300
        );
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
        Core.SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp
        );

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

        _buttonManager?.Draw(Core.SpriteBatch);

        Core.SpriteBatch.End();

        base.Draw(gameTime);
    }

    public override void Dispose()
    {
        GumService.Default.Root.Children.Clear();

        _buttonManager?.Clear();

        base.Dispose();
    }
}
