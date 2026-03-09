using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics;

public class  PhysicsWorld
{
    private List<Rigidbody> dynamicBodies;
    private List<Rigidbody> staticBodies;

    public PhysicsWorld()
    {
        dynamicBodies = new List<Rigidbody>();
        staticBodies = new List<Rigidbody>();
    }   

    public void AddBody(Rigidbody body)
    {
        if (body.IsKinematic)
        {
            staticBodies.Add(body);
        }
        else
        {
            dynamicBodies.Add(body);
        }
    }

    public void RemoveBody(Rigidbody body)
    {
        if (body.IsKinematic)
        {
            staticBodies.Remove(body);
        }
        else
        {
            dynamicBodies.Remove(body);
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var body in dynamicBodies)
        {
            body.Update(gameTime);
        }

        CheckCollisions();
    }

    public void CheckCollisions()
    {
        for (int i = 0; i < dynamicBodies.Count; i++)
        {
            for (int j = i + 1; j < dynamicBodies.Count; j++)
            {
                Rigidbody a = dynamicBodies[i];
                Rigidbody b = dynamicBodies[j];
                
                if (a.Collider.Intersects(b.Collider)) { ResolveCollision(a, b); }
            }
        }

        for (int i = 0; i < dynamicBodies.Count; i++)
        {

            for (int j = 0; j < staticBodies.Count; j++)
            {
                Rigidbody a = dynamicBodies[i];
                Rigidbody b = staticBodies[j];

                if (a.Collider.Intersects(b.Collider)) { ResolveCollision(a, b); }
            }
        }
    }
    
    public void ResolveCollision(Rigidbody a, Rigidbody b)
    {
        //Stops body from moving
        a.Velocity = new Vector2(a.Velocity.X, 0);
    }
}
