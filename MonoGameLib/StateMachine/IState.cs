using Microsoft.Xna.Framework;

namespace MonoGameLibrary.StateMachine;

public interface IState
{
    void Enter();
    void Update(GameTime gameTime);
    void Exit();
}