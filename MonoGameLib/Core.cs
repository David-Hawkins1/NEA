using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using Gum;
using Gum.Forms;
using Gum.Forms.Controls;


namespace MonoGameLibrary;

/// <summary>
/// Engine entry point. Owns the services that live for the whole game
/// (graphics, input, audio, content, virtual resolution) and runs the active scene.
/// Per-level things (PhysicsWorld, Camera, controllers) belong to Scenes, not here.
/// </summary>
public class Core : Game
{
    internal static Core s_instance;
    public static Core Instance => s_instance;

    private static Scene s_activeScene;
    private static Scene s_nextScene;

    // Virtual resolution presentation
    private RenderTarget2D _renderTarget;
    private static Rectangle s_viewport;

    // ---- Global services ----
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static new ContentManager Content { get; private set; }
    public static InputManager Input { get; private set; }
    public static AudioController Audio { get; private set; }

    /// <summary>A 1x1 white texture for drawing rectangles, debug boxes and platforms.</summary>
    public static Texture2D Pixel { get; private set; }

    // ---- Settings ----
    public static bool ExitOnEscape { get; set; }

    /// <summary>Colour the virtual screen is cleared to before the scene draws.</summary>
    public static Color ClearColor { get; set; } = Color.CornflowerBlue;

    /// <summary>Width of the game's internal (virtual) resolution.</summary>
    public static int VirtualWidth { get; private set; }

    /// <summary>Height of the game's internal (virtual) resolution.</summary>
    public static int VirtualHeight { get; private set; }

    /// <summary>Where the virtual screen is drawn inside the real window (letterboxed).</summary>
    public static Rectangle Viewport => s_viewport;

    public static Scene ActiveScene => s_activeScene;

    public Core(string title, int virtualWidth, int virtualHeight, bool fullScreen)
    {
        if (s_instance != null)
        {
            throw new InvalidOperationException("Only a single Core instance can be created");
        }

        s_instance = this;

        VirtualWidth = virtualWidth;
        VirtualHeight = virtualHeight;

        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = virtualWidth,
            PreferredBackBufferHeight = virtualHeight,
            IsFullScreen = fullScreen
        };

        Window.Title = title;
        Window.AllowUserResizing = true;

        Content = base.Content;
        Content.RootDirectory = "Content";

        IsMouseVisible = true;
        ExitOnEscape = true;
    }

    protected override void Initialize()
    {
        // The device already exists here, so set the statics BEFORE base.Initialize().
        // base.Initialize() calls LoadContent(), so anything LoadContent (or a derived
        // class) needs must already be assigned.
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Input = new InputManager();
        Audio = new AudioController();

        Window.ClientSizeChanged += (_, _) => RecalculateViewport();

        base.Initialize();
        GumService.Default.Initialize(
            this,
            DefaultVisualsVersion.V3
         );

        GumService.Default.ContentLoader.XnaContentManager = Content;

        FrameworkElement.KeyboardsForUiControl.Add(
            GumService.Default.Keyboard
        );

        FrameworkElement.GamePadsForUiControl.AddRange(
            GumService.Default.Gamepads
        );

        GumService.Default.CanvasWidth = VirtualWidth;
        GumService.Default.CanvasHeight = VirtualHeight;
    }

    protected override void LoadContent()
    {
        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White });

        _renderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);

        RecalculateViewport();

        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        s_activeScene?.Dispose();
        s_activeScene = null;

        _renderTarget?.Dispose();
        Pixel?.Dispose();
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
        Audio?.PauseAudio();
        base.OnDeactivated(sender, args);
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        Audio?.ResumeAudio();
        base.OnActivated(sender, args);
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);
        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Exit();
        }

        if (s_nextScene != null)
        {
            TransitionScene();
        }

        s_activeScene?.Update(gameTime);
        GumService.Default.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // 1. The scene draws into the fixed-size virtual screen.
        //    Scenes should call SpriteBatch.Begin/End themselves and must NOT change the render target.
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(ClearColor);

        s_activeScene?.Draw(gameTime);
        GumService.Default.Draw();

        // 2. The virtual screen is scaled and letterboxed into the real window.
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(_renderTarget, s_viewport, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    // ---- Scenes ----

    public static void ChangeScene(Scene next)
    {
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        s_activeScene?.Dispose();

        s_activeScene = s_nextScene;
        s_nextScene = null;

        s_activeScene?.Initialize();
    }

    // ---- Virtual resolution helpers ----

    /// <summary>
    /// Converts a point in real window coordinates (e.g. the mouse) into
    /// virtual-screen coordinates. Use Camera.ScreenToWorld on top of this if needed.
    /// </summary>
    public static Vector2 WindowToVirtual(Point windowPoint)
    {
        if (s_viewport.Width == 0 || s_viewport.Height == 0)
        {
            return Vector2.Zero;
        }

        float x = (windowPoint.X - s_viewport.X) / (float)s_viewport.Width * VirtualWidth;
        float y = (windowPoint.Y - s_viewport.Y) / (float)s_viewport.Height * VirtualHeight;
        return new Vector2(x, y);
    }

    private static void RecalculateViewport()
    {
        int windowWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        int windowHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        // Window minimised: nothing to calculate (also avoids dividing by zero).
        if (windowWidth <= 0 || windowHeight <= 0)
        {
            return;
        }

        float targetAspect = VirtualWidth / (float)VirtualHeight;
        float windowAspect = windowWidth / (float)windowHeight;

        int width;
        int height;

        if (windowAspect > targetAspect)
        {
            height = windowHeight;
            width = (int)(windowHeight * targetAspect);
        }
        else
        {
            width = windowWidth;
            height = (int)(windowWidth / targetAspect);
        }

        s_viewport = new Rectangle(
            (windowWidth - width) / 2,
            (windowHeight - height) / 2,
            width,
            height);
    }
}
