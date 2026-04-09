using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;

public class Rigidbody
{
    // Physics properties

    // The Position of the object. Centre.
    public Vector2 Position { get; set; }
    // The Velocity of the object. Speed and direction.
    public Vector2 Velocity { get; set; }
    // The speed change of the object.
    public Vector2 Acceleration { get; set; }
    // The mass of the object.
    public float Mass { get; set; } = 1.0f;
    // How much the object is affected by gravity.
    public float GravityScale { get; set; } = 1.0f;
    // Is the object immovable and unaffected by forces?
    public bool IsKinematic { get; set; } = false;
    // Is the object affected by gravity?
    public bool UseGravity { get; set; } = true;


    // Forces/Damping properties

    // The drag applied to the object.
    public float Drag { get; set; } = 0f;
    public float Friction { get; set; } = 0f;
    // Gravity Constant
    public float Gravity { get; set; } = 1200f;
    // Temporary force application value
    private Vector2 _appliedForce = Vector2.Zero;
    public float MaxSpeed = 600f;
    public float MaxFallSpeed = 1000f;

    //Collider properties
    public float Height { get; private set; }
    public float Width { get; private set; }

    // Platformer properties
    public bool IsGrounded { get; set; }
    public bool IsTouchingWall { get; set; }

    // Constructors
    public Rigidbody(Vector2 position, float width, float height)
    {
        Position = position;
        Height = height;
        Width = width;
    }
    public Rigidbody(Vector2 position, float width, float height, float gravityScale)
    {
        Position = position;
        Height = height;
        Width = width;
        GravityScale = gravityScale;
    }


    // Base Methods
    public void Update(GameTime gameTime)
    {
        if (IsKinematic) { return; }
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (UseGravity)
        {
            Acceleration += new Vector2(0,Gravity * GravityScale);
        }
        Velocity += Acceleration * deltaTime;
        Velocity += (_appliedForce / Mass) * deltaTime;
        if (IsGrounded) { Velocity *= 1f - (Friction * deltaTime);}
        else { Velocity *= 1f - (Drag * deltaTime); }
        
        if (Velocity.Length() > MaxSpeed)
        {
            Velocity = Vector2.Normalize(Velocity) * MaxSpeed;
        }
        Position += Velocity * deltaTime;
        Acceleration = Vector2.Zero;
        _appliedForce = Vector2.Zero;
        if (Velocity.Y > MaxFallSpeed)
        {
            Velocity = new Vector2(Velocity.X, MaxFallSpeed);
        }
    }
    public void AddForce(Vector2 force)
    {
        _appliedForce += force;
    }
    public void AddImpulse(Vector2 impulse)
    {
        Velocity += impulse / Mass;
    }
    public void SetVelocity(Vector2 velocity)
    {
        Velocity = velocity;
    }
    public void AddVelocity(Vector2 velocity)
    {
        Velocity += velocity;
    }

    // Utility Methods
    public void ResetForces()
    {
        Acceleration = Vector2.Zero;
        _appliedForce = Vector2.Zero;
    }

    //Collision Methods
    public Rectangle Bounds
    {
        get
        {
            return new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height
            );
        }
    }
    public void StopX()
    {
        Velocity = new Vector2(0, Velocity.Y);
    }

    public void StopY()
    {
        Velocity = new Vector2(Velocity.X, 0);
    }
}
