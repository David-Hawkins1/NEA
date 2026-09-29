using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Physics.Collision;

namespace MonoGameLibrary.Physics;

public class CollisionManager
{
    public void ResolveCol(PhysicsObject obj, List<Rectangle> worldColliders)
    {
        obj.Rigidbody.IsGrounded = false;
        obj.Rigidbody.IsTouchingWall = false;

        foreach (var tile in worldColliders)
        {
            Resolve(obj, tile);
        }
    }

    private void Resolve(PhysicsObject obj, Rectangle tile)
    {
        Rectangle overlap = Rectangle.Intersect(obj.Bounds, tile);
        if (overlap.Width <= 0 || overlap.Height <= 0)
        {
            return;
        }

        Rigidbody rb = obj.Rigidbody;

        if (overlap.Width < overlap.Height)
        {
            // Horizontal collision
            if (rb.Position.X < tile.Center.X)
                rb.Position -= new Vector2(overlap.Width, 0);
            else
                rb.Position += new Vector2(overlap.Width, 0);

            rb.StopX();
            rb.IsTouchingWall = true;
        }
        else
        {
            // Vertical collision
            if (rb.Position.Y < tile.Center.Y)
            {
                rb.Position -= new Vector2(0, overlap.Height);
                rb.IsGrounded = true;
            }
            else
            {
                rb.Position += new Vector2(0, overlap.Height);
            }

            rb.StopY();
        }
    }
    public SweepResult Sweep(PhysicsObject obj, Rectangle target, Vector2 movement)
{
    Rectangle bounds = obj.Bounds;

    if (bounds.Intersects(target))
    {
        float left = target.Right - bounds.Left;
        float right = bounds.Right - target.Left;
        float top = target.Bottom - bounds.Top;
        float bottom = bounds.Bottom - target.Top;

        float horizontalPenetration =
            MathF.Min(left, right);

        float verticalPenetration =
            MathF.Min(top, bottom);

        Vector2 collisionNormal;

        if (verticalPenetration < horizontalPenetration)
        {
            collisionNormal = movement.Y >= 0
                ? new Vector2(0, -1)
                : new Vector2(0, 1);
        }
        else
        {
            collisionNormal = movement.X >= 0
                ? new Vector2(-1, 0)
                : new Vector2(1, 0);
        }

        return new SweepResult
        {
            Hit = true,
            Time = 0f,
            Normal = collisionNormal
        };
    }

    if (movement == Vector2.Zero)
    {
        return new SweepResult
        {
            Hit = false,
            Time = 1f,
            Normal = Vector2.Zero
        };
    }

    if (movement.X == 0 &&
        (bounds.Right <= target.Left ||
         bounds.Left >= target.Right))
    {
        return new SweepResult
        {
            Hit = false,
            Time = 1f,
            Normal = Vector2.Zero
        };
    }

    if (movement.Y == 0 &&
        (bounds.Bottom <= target.Top ||
         bounds.Top >= target.Bottom))
    {
        return new SweepResult
        {
            Hit = false,
            Time = 1f,
            Normal = Vector2.Zero
        };
    }

    float xEntry;
    float xExit;
    float yEntry;
    float yExit;

    if (movement.X > 0)
    {
        xEntry = target.Left - bounds.Right;
        xExit = target.Right - bounds.Left;
    }
    else if (movement.X < 0)
    {
        xEntry = target.Right - bounds.Left;
        xExit = target.Left - bounds.Right;
    }
    else
    {
        xEntry = float.NegativeInfinity;
        xExit = float.PositiveInfinity;
    }

    if (movement.Y > 0)
    {
        yEntry = target.Top - bounds.Bottom;
        yExit = target.Bottom - bounds.Top;
    }
    else if (movement.Y < 0)
    {
        yEntry = target.Bottom - bounds.Top;
        yExit = target.Top - bounds.Bottom;
    }
    else
    {
        yEntry = float.NegativeInfinity;
        yExit = float.PositiveInfinity;
    }

    float xEntryTime =
        movement.X == 0
            ? float.NegativeInfinity
            : xEntry / movement.X;

    float xExitTime =
        movement.X == 0
            ? float.PositiveInfinity
            : xExit / movement.X;

    float yEntryTime =
        movement.Y == 0
            ? float.NegativeInfinity
            : yEntry / movement.Y;

    float yExitTime =
        movement.Y == 0
            ? float.PositiveInfinity
            : yExit / movement.Y;

    float entryTime =
        MathF.Max(xEntryTime, yEntryTime);

    float exitTime =
        MathF.Min(xExitTime, yExitTime);

    if (entryTime > exitTime ||
        entryTime < 0f ||
        entryTime > 1f)
    {
        return new SweepResult
        {
            Hit = false,
            Time = 1f,
            Normal = Vector2.Zero
        };
    }

    Vector2 normal;

    if (xEntryTime > yEntryTime)
    {
        normal = movement.X > 0
            ? new Vector2(-1, 0)
            : new Vector2(1, 0);
    }
    else
    {
        normal = movement.Y > 0
            ? new Vector2(0, -1)
            : new Vector2(0, 1);
    }

    return new SweepResult
    {
        Hit = true,
        Time = entryTime,
        Normal = normal
    };
}
}