
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Physics;
using MonoGameLibrary;
using RenderingLibrary.Math;

namespace NEA.Controllers;

public class PlayerController
{
    // Managers and the player object
    private readonly PhysicsObject _player;
    private readonly InputManager _input;

    // Movement constants
    private const float GroundAcceleration = 1800f;
    private const float AirAcceleration = 900f;
    private const float TurnAcceleration = 6000f;
    private const float GroundDeceleration = 1500f;
    private const float AirDeceleration = 750f;
    private const float JumpForce = 950f;

    // Time consts

    private const float CoyoteTime = 0.1f;
    private const float JumpBufferTime = 0.1f;
    // Timers
    private readonly Timer _coyoteTimer;
    private readonly Timer _jumpBufferTimer;

    private bool _wasGrounded;

    public PlayerController(PhysicsObject player, InputManager input) // Constructor
    {
        _player = player;
        _input = input;

        _coyoteTimer = new Timer(CoyoteTime);
        _jumpBufferTimer = new Timer(JumpBufferTime);

    }

    public void Update(GameTime gameTime) //Update method
    {
        float deltaTime =
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        _coyoteTimer.Update(gameTime);
        _jumpBufferTimer.Update(gameTime);

        HandleJump(gameTime);
        HandleMovement(deltaTime);
    }

    private void HandleMovement(float deltaTime) // Handles a d movement
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
            float deceleration = rb.IsGrounded
                ? GroundDeceleration
                : AirDeceleration;

            MoveTowardsZero(deceleration, deltaTime);
        }
    }
    private void HandleJump(GameTime gameTime)
    {
        var keyboard = _input.Keyboard;
        Rigidbody rb = _player.Rigidbody;

        bool isGrounded = rb.IsGrounded;

        // Start coyote time when leaving the ground.
        if (_wasGrounded && !isGrounded)
        {
            _coyoteTimer.Start();
        }

        // Refresh coyote time while grounded.
        if (isGrounded)
        {
            _coyoteTimer.Start();
        }

        // Remember a jump press briefly.
        if (keyboard.WasKeyJustPressed(Keys.Space))
        {
            _jumpBufferTimer.Start();
        }

        // Jump if grounded (or within coyote time) and buffered.
        if (_jumpBufferTimer.IsRunning &&
            (isGrounded || _coyoteTimer.IsRunning))
        {
            rb.Velocity = new Vector2(
                rb.Velocity.X,
                -JumpForce);

            _jumpBufferTimer.Stop();
            _coyoteTimer.Stop();
        }

        _wasGrounded = isGrounded;
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
