using Microsoft.Xna.Framework;

namespace MonoGameLibrary.StateMachine;

public class StateMachine
{
    public IState CurrentState { get; private set; }
    public bool HasState => CurrentState != null;

    public void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update(GameTime gameTime)
    {
        CurrentState?.Update(gameTime);
    }
}