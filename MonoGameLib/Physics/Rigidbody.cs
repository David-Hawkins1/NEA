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
    // Gravity Constant
    public float Gravity { get; set; } = 1200f;
    // Temporary force application value
    private Vector2 _appliedForce = Vector2.Zero;
    public float MaxSpeed = 600f;


    // Collider
    public Collider Collider { get; private set; }

    // Constructors
    public Rigidbody(Vector2 position, float width, float height)
    {
        Position = position;
        Collider = new Collider(this, width, height);
    }
    public Rigidbody(Vector2 position, float width, float height, float gravityScale)
    {
        Position = position;
        Collider = new Collider(this, width, height);
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
        Velocity *= 1f - (Drag * deltaTime);
        Velocity = Vector2.Clamp(Velocity, new Vector2(-MaxSpeed, -MaxSpeed), new Vector2(MaxSpeed, MaxSpeed));
        Position += Velocity * deltaTime;
        Acceleration = Vector2.Zero;
        _appliedForce = Vector2.Zero;
    }
    public void AddForce(Vector2 force)
    {
        _appliedForce += force;
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
}
