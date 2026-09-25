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
        if (movement == Vector2.Zero) // If not moving collision is not happening
        {
            return new SweepResult
            {
                Hit = false,
                Time = 1f,
                Normal = Vector2.Zero
            };
        }

        float xEntry; float xExit; float yEntry; float yExit;
        // Finding distances between the box entering target collider and exiting
        if (movement.X > 0) { xEntry = target.Left - bounds.Right; xExit = target.Right - bounds.Left;}
        else { xEntry = target.Right - bounds.Left; xExit = target.Left - bounds.Right;}

        if (movement.Y > 0) { yEntry = target.Top - bounds.Bottom; yExit = target.Bottom - bounds.Top;}
        else  { yEntry = target.Bottom - bounds.Top; yExit = target.Top - bounds.Bottom;}

        // Distances => Times
        float xEntryTime; float xExitTime; float yEntryTime; float yExitTime;

        if (movement.X == 0) { xEntryTime = float.NegativeInfinity; xExitTime = float.PositiveInfinity;}
        else { xEntryTime = xEntry / movement.X; xExitTime = xExit / movement.X;}

        if (movement.Y == 0) { yEntryTime = float.NegativeInfinity; yExitTime = float.PositiveInfinity;}
        else { yEntryTime = yEntry / movement.Y; yExitTime = yExit / movement.Y;}

        float entryTime = MathF.Max(xEntryTime, yEntryTime);
        float exitTime = MathF.Min(xExitTime, yExitTime);

        if (entryTime > exitTime || entryTime < 0f || entryTime > 1f)
        {
            return new SweepResult { Hit = false, Time = 1f, Normal = Vector2.Zero};
        }

        Vector2 normal;
        if (xEntryTime > yEntryTime) { normal = movement.X > 0 ? new Vector2(-1,0) : new Vector2(1,0); }
        else { normal = movement.Y > 0 ? new Vector2(0, -1) : new Vector2(0,1); }

        return new SweepResult { Hit = true, Time = entryTime, Normal = normal };
    }
}