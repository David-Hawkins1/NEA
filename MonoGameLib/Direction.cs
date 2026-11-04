using Microsoft.Xna.Framework;
using System;

namespace MonoGameLib
{
    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    public static class DirectionHelper
    {
        public static Vector2 GetVector(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Vector2(0, -1),
                Direction.NorthEast => Vector2.Normalize(new Vector2(1, -1)),
                Direction.East => new Vector2(1, 0),
                Direction.SouthEast => Vector2.Normalize(new Vector2(1, 1)),
                Direction.South => new Vector2(0, 1),
                Direction.SouthWest => Vector2.Normalize(new Vector2(-1, 1)),
                Direction.West => new Vector2(-1, 0),
                Direction.NorthWest => Vector2.Normalize(new Vector2(-1, -1)),
                _ => Vector2.Zero
            };
        }
        public static Direction GetDirection(Vector2 vector)
        {
            if (vector == Vector2.Zero)
                throw new ArgumentException("Direction vector cannot be zero.");

            float angle = MathF.Atan2(vector.Y, vector.X);

            // Convert radians to degrees
            float degrees = MathHelper.ToDegrees(angle);

            // Round to the nearest 45 degrees
            int octant = (int)MathF.Round(degrees / 45f);

            // Convert to a positive range (0–7)
            octant = (octant + 8) % 8;

            return octant switch
            {
                0 => Direction.East,
                1 => Direction.SouthEast,
                2 => Direction.South,
                3 => Direction.SouthWest,
                4 => Direction.West,
                5 => Direction.NorthWest,
                6 => Direction.North,
                7 => Direction.NorthEast,
                _ => Direction.East
            };
        }
        public static Direction GetOpposite(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.NorthEast => Direction.SouthWest,
                Direction.East => Direction.West,
                Direction.SouthEast => Direction.NorthWest,
                Direction.South => Direction.North,
                Direction.SouthWest => Direction.NorthEast,
                Direction.West => Direction.East,
                Direction.NorthWest => Direction.SouthEast,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }
    }
}
