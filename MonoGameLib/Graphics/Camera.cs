using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class Camera
{
    public Vector2 Position = Vector2.Zero;
    public float Zoom = 1.0f;
    public float Rotation = 0.0f;
    public Rectangle Bounds = Rectangle.Empty;
    public Rectangle? LevelBounds = null;
    private Vector2 _shakeOffset = Vector2.Zero;
    private float _shakeDuration = 0.0f;
    private float _shakeMagnitude = 0.0f;

    public Camera(int width, int height)
    {
        Bounds = new Rectangle(0, 0, width, height);
    }

    public Matrix GetViewMatrix()
    {
        return Matrix.CreateTranslation(new Vector3(-Position + _shakeOffset, 0.0f)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(Zoom, Zoom, 1.0f) *
               Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0.0f));
    }

    public void Follow(Vector2 target, float lerpSpeed, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position = Vector2.Lerp(Position, target, lerpSpeed * dt);
    }

    public void ClampToBounds()
    {
        if (LevelBounds == null)
            return;

        Rectangle levelBounds = LevelBounds.Value;
        float halfWidth = (Bounds.Width / Zoom)* 0.5f;
        float halfHeight = (Bounds.Height / Zoom)* 0.5f;

        float clampX = MathHelper.Clamp(Position.X, levelBounds.Left + halfWidth, levelBounds.Right - halfWidth);
        float clampY = MathHelper.Clamp(Position.Y, levelBounds.Top + halfHeight, levelBounds.Bottom - halfHeight);

        Position = new Vector2(clampX, clampY);
    }

    public void Shake(float duration, float magnitude)
    {
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
    }

    public void UpdateShake(GameTime gameTime)
    {
        Random _random = new Random();
        if (_shakeDuration <=0)
        {
            _shakeOffset = Vector2.Zero;
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _shakeDuration -= dt;
        float x = (float)(_random.NextDouble()  * 2 - 1) * _shakeMagnitude;
        float y = (float)(_random.NextDouble()  * 2 - 1) * _shakeMagnitude;
        _shakeOffset = new Vector2(x, y);
    }

}