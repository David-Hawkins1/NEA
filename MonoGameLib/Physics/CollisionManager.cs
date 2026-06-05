using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;
public class CollisionManager
{
    public void ResolveCol(Rigidbody rb, List<Rectangle> worldColliders)
    {
        rb.IsGrounded = false;
        rb.IsTouchingWall = false;
        foreach (var collider in worldColliders)
        {
            Resolve(rb, collider);
        }

    }

    private void Resolve(Rigidbody rb, Rectangle tile)
    {
        Rectangle overlap = Rectangle.Intersect(rb.Bounds, tile);
        if (overlap.Width <= 0 || overlap.Height <= 0)
        {
            return;
        }
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