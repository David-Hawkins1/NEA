using MonoGameLibrary;
using NEA.Scenes;

namespace NEA;

public class Game1 : Core
{
    public Game1() : base("The Trapped Lily", 1920, 1080, false)
    {
    }

    protected override void Initialize()
    {
        // Sets up Core's services (input, audio, sprite batch, render target).
        base.Initialize();

        ChangeScene(new StartScene());
    }
}