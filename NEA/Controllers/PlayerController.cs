

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Physics;
using MonoGameLibrary;

namespace NEA.Controllers;

public class PlayerController
{
    private readonly PhysicsObject _player;
    private readonly InputManager _input;

    // Movement
    private const float GroundAcceleration = 1800f;
    private const float AirAcceleration = 900f;
    private const float TurnAcceleration = 6000f;
    private const float GroundDeceleration = 1500f;
    private const float AirDeceleration = 750f;
    private const float JumpForce = 950f;

    // Jump timing
    private const float CoyoteTime = 0.1f;
    private const float JumpBufferTime = 0.1f;

    private float _coyoteRemaining = 0f;
    private float _jumpBufferRemaining = 0f;

    private bool _wasGrounded;
    private bool _jumpAvailable = true;

    public PlayerController(PhysicsObject player, InputManager input)
    {
        _player = player;
        _input = input;
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime =
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Count both timers down exactly once per frame.
        _coyoteRemaining = MathHelper.Max(
            0f, _coyoteRemaining - deltaTime);

        _jumpBufferRemaining = MathHelper.Max(
            0f, _jumpBufferRemaining - deltaTime);

        HandleJump();

        HandleMovement(deltaTime);
    }

    private void HandleJump()
    {
        var keyboard = _input.Keyboard;
        Rigidbody rb = _player.Rigidbody;

        bool isGrounded = rb.IsGrounded;

        // Start the buffer only when Space is newly pressed.
        if (keyboard.WasKeyJustPressed(Keys.Space))
        {
            _jumpBufferRemaining = JumpBufferTime;
        }

        // Detect landing: restore the jump only on the
        // transition from airborne to grounded.
        if (isGrounded && !_wasGrounded)
        {
            _jumpAvailable = true;
        }

        // Start coyote time when leaving the ground,
        // provided the player hasn't already jumped.
        if (!isGrounded && _wasGrounded && _jumpAvailable)
        {
            _coyoteRemaining = CoyoteTime;
        }

        // Jump only while the buffer is active and
        // a ground jump or coyote jump is available.
        if (_jumpBufferRemaining > 0f &&
            _jumpAvailable &&
            (isGrounded || _coyoteRemaining > 0f))
        {
            rb.Velocity = new Vector2(
                rb.Velocity.X,
                -JumpForce);

            _jumpAvailable = false;
            _jumpBufferRemaining = 0f;
            _coyoteRemaining = 0f;
        }

        _wasGrounded = isGrounded;
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
            float deceleration = rb.IsGrounded
                ? GroundDeceleration
                : AirDeceleration;

            MoveTowardsZero(deceleration, deltaTime);
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

