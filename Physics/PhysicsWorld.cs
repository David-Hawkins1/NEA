using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics
{
    public class PhysicsWorld
    {
        private readonly List<PhysicsObject> _objects;
        private readonly CollisionManager _collisionManager;

        public PhysicsWorld()
        {
            _objects = new List<PhysicsObject>();
            _collisionManager = new CollisionManager();
        }
        public void Add(PhysicsObject physicsObject)
        {
            if (physicsObject == null)
                throw new ArgumentNullException(nameof(physicsObject));
            _objects.Add(physicsObject);
        }
        public void Remove(PhysicsObject physicsObject)
        {
            _objects.Remove(physicsObject);
        }

        public void Step(GameTime gameTime) 
        {
            foreach (PhysicsObject obj in _objects)
            {
                obj.Update(gameTime);
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 movement = obj.Rigidbody.Velocity * deltaTime;

                // Swept

                obj.Rigidbody.Position += movement;
            }
        }
    }
}
