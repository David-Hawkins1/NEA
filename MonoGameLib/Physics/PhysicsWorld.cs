using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Physics.Collision;

namespace MonoGameLibrary.Physics;

public class PhysicsWorld
{
    private readonly List<PhysicsObject> _objects;
    private readonly List<Rectangle> _worldColliders;
    private readonly CollisionManager _collisionManager;

    public PhysicsWorld()
    {
        _objects = new List<PhysicsObject>();
        _worldColliders = new List<Rectangle>();
        _collisionManager = new CollisionManager();
    }

    public void Add(PhysicsObject physicsObject)
    {
        if (physicsObject == null)
            throw new ArgumentNullException(nameof(physicsObject));

        _objects.Add(physicsObject);
    }

    public void Remove(PhysicsObject physicsObject)
    {
        _objects.Remove(physicsObject);
    }

    public void AddCollider(Rectangle collider)
    {
        _worldColliders.Add(collider);
    }

    public void Step(GameTime gameTime)
    {
        float deltaTime =
            (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (PhysicsObject obj in _objects)
        {
            Rigidbody rb = obj.Rigidbody;

            if (rb.IsKinematic)
                continue;

            rb.IsGrounded = false;
            rb.IsTouchingWall = false;

            obj.Update(gameTime);

            Vector2 remainingMovement =
                rb.Velocity * deltaTime;

            for (int iteration = 0; iteration < 4; iteration++)
            {
                if (remainingMovement == Vector2.Zero)
                    break;

                SweepResult closestHit = new SweepResult
                {
                    Hit = false,
                    Time = 1f,
                    Normal = Vector2.Zero
                };

                foreach (Rectangle collider in _worldColliders)
                {
                    SweepResult result =
                        _collisionManager.Sweep(
                            obj,
                            collider,
                            remainingMovement
                        );

                    if (result.Hit &&
                        result.Time < closestHit.Time)
                    {
                        closestHit = result;
                    }
                }

                if (!closestHit.Hit)
                {
                    rb.Position += remainingMovement;
                    break;
                }

                float hitTime =
                    MathHelper.Clamp(closestHit.Time, 0f, 1f);

                rb.Position +=
                    remainingMovement * hitTime;

                Vector2 normal = closestHit.Normal;

                if (normal.Y < 0)
                {
                    rb.IsGrounded = true;
                    rb.StopY();
                }
                else if (normal.Y > 0)
                {
                    rb.StopY();
                }

                if (normal.X != 0)
                {
                    rb.IsTouchingWall = true;
                    rb.StopX();
                }

                remainingMovement *= 1f - hitTime;

                if (normal.X != 0)
                    remainingMovement.X = 0;

                if (normal.Y != 0)
                    remainingMovement.Y = 0;

                rb.Position += normal * 0.01f;
            }
        }
    }
}
