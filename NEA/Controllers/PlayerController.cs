
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Physics;

namespace NEA.Controllers;

public class PlayerController
{
    private readonly PhysicsObject _player;
    private readonly InputManager _input;

    private const float GroundAcceleration = 1800f;
    private const float AirAcceleration = 900f;
    private const float TurnAcceleration = 6000f;
    private const float Deceleration = 900f;

    public PlayerController(PhysicsObject player, InputManager input)
    {
        _player = player;
        _input = input;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime =
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        HandleMovement(deltaTime);
    }

    private void HandleMovement(float deltaTime)
    {
        var keyboard = _input.Keyboard;
        Rigidbody rb = _player.Rigidbody;

        float direction = 0f;

        if (keyboard.IsKeyDown(Keys.A))
            direction -= 1f;

        if (keyboard.IsKeyDown(Keys.D))
            direction += 1f;

        if (direction != 0f)
        {
            float acceleration = rb.IsGrounded
                ? GroundAcceleration
                : AirAcceleration;

            if ((direction > 0f && rb.Velocity.X < 0f) ||
                (direction < 0f && rb.Velocity.X > 0f))
            {
                acceleration = TurnAcceleration;
            }

            Move(direction, acceleration, deltaTime);
        }
        else
        {
            MoveTowardsZero(Deceleration, deltaTime);
        }
    }

    private void Move(
        float direction,
        float acceleration,
        float deltaTime)
    {
        Rigidbody rb = _player.Rigidbody;

        float targetSpeed = direction * rb.MaxGroundSpeed;
        float change = MathHelper.Clamp(
            targetSpeed - rb.Velocity.X,
            -acceleration * deltaTime,
            acceleration * deltaTime);

        rb.Velocity = new Vector2(
            rb.Velocity.X + change,
            rb.Velocity.Y);
    }

    private void MoveTowardsZero(
        float deceleration,
        float deltaTime)
    {
        Rigidbody rb = _player.Rigidbody;

        float change = MathHelper.Clamp(
            -rb.Velocity.X,
            -deceleration * deltaTime,
            deceleration * deltaTime);

        rb.Velocity = new Vector2(
            rb.Velocity.X + change,
            rb.Velocity.Y);
    }
}
