using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;

public class Collider
{
    public Rigidbody Body { get; private set; }

    public float Width { get; set; }
    public float Height { get; set; }

    public Collider(Rigidbody body, float width, float height)
    {
        Body = body;
        Width = width;
        Height = height;
    }

    public Rectangle Bounds
    {
        get
        {
            return new Rectangle(
                (int)(Body.Position.X - Width / 2),
                (int)(Body.Position.Y - Height / 2),
                (int)Width,
                (int)Height
            );
        }
    }

    public bool Intersects(Collider other)
    {
        return Bounds.Intersects(other.Bounds);
    }
} 