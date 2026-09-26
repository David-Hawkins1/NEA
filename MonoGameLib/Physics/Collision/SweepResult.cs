using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Physics.Collision
{
    public struct SweepResult
    {
        public bool Hit { get; set; }
        public float Time { get; set; }
        public Vector2 Normal { get; set; }
    }
}
