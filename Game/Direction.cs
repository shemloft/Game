using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public enum Direction
    {
        Right,
        Left,
        Up,
        Down,
        NoDirection
    }

    public class Directions
    {
        public static Vector[] SimpleAround =
        {
            Vector.Up,
            Vector.Up.Rotate(Math.PI / 6),
            Vector.Up.Rotate(Math.PI / 3),
            Vector.Up.Rotate(Math.PI / 2),
            Vector.Up.Rotate(2 * Math.PI / 3),
            Vector.Up.Rotate(5 * Math.PI / 6),
            Vector.Up.Rotate(Math.PI),
            Vector.Up.Rotate(7 * Math.PI / 6),
            Vector.Up.Rotate(4 * Math.PI / 3),
            Vector.Up.Rotate(3 * Math.PI / 2),
            Vector.Up.Rotate(5 * Math.PI / 3),
            Vector.Up.Rotate(11 * Math.PI / 6)
        };

        public static Func<int, Vector[]> GetAroundVectors = x =>
        {
            var result = new List<Vector> { Vector.Up };
            for (var i = Math.PI / x; i < 2 * Math.PI; i += Math.PI / x)
            {
                result.Add(Vector.Up.Rotate(i));
            }
            return result.ToArray();
        };

        public static Func<int, double, Vector[]> GetRow = (count, y) =>
        {
            var result = new List<Vector>();
            for (var i = Game.GameFeatures.FieldSize.Width / (count + 1); i < Game.GameFeatures.FieldSize.Width;
                i += Game.GameFeatures.FieldSize.Width / (count + 1))
                result.Add(new Vector(i, y));
            return result.ToArray();
        };
    }

}
