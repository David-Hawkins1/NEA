using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;

public class Collider
{
    //Collider Properties
    public float Width {get; set;}
    public float Height {get; set;}

    // Constructor
    public Collider(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public Rectangle GetBounds(Vector2 position)
    {
        return new Rectangle(
            (int)(position.X - Width /2),
            (int)(position.Y - Height /2),
            (int)Width,
            (int)Height
        );
    }
}