using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
}