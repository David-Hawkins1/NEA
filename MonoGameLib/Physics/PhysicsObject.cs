using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;

public class PhysicsObject
{
        // Handles position, velocity, and forces for this object.
    public Rigidbody Rigidbody { get; }


    // Handles the shape/size used for collision for this object.
    public Collider Collider { get; }

    // Gets the current center position of this object.
    public Vector2 Position => Rigidbody.Position;

    // Gets the current axis-aligned bounding box of this object.
    public Rectangle Bounds => Collider.GetBounds(Rigidbody.Position);

    public PhysicsObject(Vector2 position, float width, float height)
    {
        Rigidbody = new Rigidbody(position);
        Collider = new Collider(width, height);
    }

    public PhysicsObject(Vector2 position, float width, float height, float gravityScale)
    {
        Rigidbody = new Rigidbody(position, gravityScale);
        Collider = new Collider(width, height);
    }

    public void Update(GameTime gameTime)
    {
        Rigidbody.Update(gameTime);
    }
}