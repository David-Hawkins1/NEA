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
    public float MaxGroundSpeed { get; set; } = 600f;
    public float MaxAirSpeed { get; set; } = 450f;
    public float MaxFallSpeed { get; set; } = 1000f;    

    // Platformer properties
    public bool IsGrounded { get; set; }
    public bool IsTouchingWall { get; set; }

    // Constructors
    public Rigidbody(Vector2 position)
    {
        Position = position;
    }
    public Rigidbody(Vector2 position, float gravityScale)
    {
        Position = position;
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
        if (IsGrounded)
        {
            float frictionFactor =
                Math.Max(0f, 1f - Friction * deltaTime);
            Velocity *= frictionFactor;
        }
        else
        {
            float dragFactor =
                Math.Max(0f, 1f - Drag * deltaTime);
            Velocity =new Vector2(Velocity.X * dragFactor, Velocity.Y);
        }
        
        Velocity = new Vector2(
            MathHelper.Clamp(Velocity.X, -MaxGroundSpeed, MaxGroundSpeed),
            MathHelper.Clamp(Velocity.Y, -MaxFallSpeed, MaxFallSpeed)
            );
        Position += Velocity * deltaTime; //Semi-Implicit Euler Integration
        Acceleration = Vector2.Zero;
        _appliedForce = Vector2.Zero;
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
    public void StopX()
    {
        Velocity = new Vector2(0, Velocity.Y);
    }

    public void StopY()
    {
        Velocity = new Vector2(Velocity.X, 0);
    }
}
