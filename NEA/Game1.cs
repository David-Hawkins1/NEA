using MonoGameLibrary;
using NEA.Scenes;

namespace NEA;

public class Game1 : Core
{
    public Game1() : base("NEA", 1600, 1200, false)
    {
    }

    protected override void Initialize()
    {
        // Sets up Core's services (input, audio, sprite batch, render target).
        base.Initialize();

        ChangeScene(new StartScene());
    }
}