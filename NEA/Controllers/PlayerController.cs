using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Physics;
using MonoGameLibrary;
using System.ComponentModel.Design.Serialization;
using MonoGameLib;
using Gum.DataTypes.Variables;

namespace NEA.Controllers;

public class PlayerController
{
    private readonly PhysicsObject _player;
    private readonly InputManager _input;

    // Movement
    private const float GroundAcceleration = 1800f;
    private const float AirAcceleration = 900f;
    private const float TurnAcceleration = 6000f;
    private const float GroundDeceleration = 2000f;
    private const float AirDeceleration = 500f;
    private const float JumpForce = 950f;

    // Jump timing
    private const float CoyoteTime = 0.075f;
    private const float JumpBufferTime = 0.08f;

    private float _coyoteRemaining = 0f;
    private float _jumpBufferRemaining = 0f;

    private bool _wasGrounded;
    private bool _jumpAvailable = true;

    // Dash variables
    private const float DashDuration = 0.15f;
    private const float DashSpeed = 1200f;
    private float _dashRemaining = 0f;
    private bool _isDashing = false;
    private Direction _LastDir = Direction.East;

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

        UpdateLastDirection();

        HandleDash(deltaTime);
        HandleJump();
        if (_isDashing)
        {
            return;
        }
        
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
    private void HandleDash(float deltaTime)
    {
        var keyboard = _input.Keyboard;
         if (keyboard.WasKeyJustPressed(Keys.LeftShift) && !_isDashing)
         {
            StartDash();
         }
        else if (!_isDashing)
        {
            return;
        }
        _dashRemaining -= deltaTime;

        if (_dashRemaining <= 0f)
        {
            EndDash();
        }
    }
    private void StartDash()
    {
        _isDashing = true;
        _dashRemaining = DashDuration;
        Vector2 dashDirection = DirectionHelper.GetVector(_LastDir);

        _player.Rigidbody.IgnoreSpeedLimits = true;
        _player.Rigidbody.UseGravity = false;
        _player.Rigidbody.Velocity = dashDirection * DashSpeed;
    }
    private void EndDash()
    {
        _isDashing = false;
        _player.Rigidbody.IgnoreSpeedLimits = false;
        _player.Rigidbody.UseGravity = true;
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
    private void UpdateLastDirection()
    {
        var keyboard = _input.Keyboard;

        int x = 0;
        int y = 0;

        if (keyboard.IsKeyDown(Keys.A))
            x--;

        if (keyboard.IsKeyDown(Keys.D))
            x++;

        if (keyboard.IsKeyDown(Keys.W))
            y--;

        if (keyboard.IsKeyDown(Keys.S))
            y++;

        if (x == 0 && y == 0)
            return;

        Vector2 direction = new Vector2(x, y);

        _LastDir = DirectionHelper.GetDirection(direction);
    }
}

