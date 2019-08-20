using System;
using System.Linq;

namespace Game
{
    public interface IMovingPattern
    {
        Vector Move(double step);
    }

    public class Immobile : IMovingPattern
    {
        public Vector Location;

        public Immobile(Vector location)
        {
            Location = location;
        }

        public Vector Move(double step)
        {
            return Location;
        }
    }

    public class VectorMove : IMovingPattern
    {
        public Vector Location;
        private readonly Vector[] moves;
        private readonly int frequency;
        private int moveCount;
        private int iteration;

        public VectorMove(Vector location, int frequency, params Vector[] moves)
        {
            Location = location;
            this.frequency = frequency;
            if (moves.Length == 1) this.frequency = 0;
            this.moves = moves.Select(v => v.Normalize()).ToArray();
        }

        public Vector Move(double step)
        {
            Location += moves[moveCount] * step;
            iteration++;

            if (iteration == frequency * (moveCount + 1)) moveCount++;

            if (iteration == frequency * moves.Length || moves.Length == 1)
            {
                iteration = 0;
                moveCount = 0;
            }

            return Location;
        }
    }

    public class MoveToTarget : IMovingPattern
    {
        public Vector Location;
        private Vector target;
        private Vector direction;

        public MoveToTarget(Vector location, Vector target)
        {
            Location = location;
            this.target = target;
            direction = target - location;
        }

        public Vector Move(double step)
        {
            if (Location.EqualWithInfelicity(target, step))
                return Location;

            Location += step * direction;
            return Location;
        }
    }

    public class VectorMoves
    {
        public static Func<Vector, IMovingPattern> MoveHalfCircleRight = location => new VectorMove(location,
            10, Vector.Down,
            Vector.Right.Rotate(Math.PI / 3),
            Vector.Right.Rotate(Math.PI / 4),
            Vector.Right.Rotate(Math.PI / 5),
            Vector.Right.Rotate(Math.PI / 6),
            Vector.Right.Rotate(Math.PI / 7),
            Vector.Right.Rotate(Math.PI / 8),
            Vector.Right.Rotate(Math.PI / 9),
            Vector.Right.Rotate(Math.PI / 10),
            Vector.Right);

        public static Func<Vector, IMovingPattern> MoveHalfCircleLeft = location => new VectorMove(location,
            10, Vector.Down,
            Vector.Left.Rotate(-Math.PI / 3),
            Vector.Left.Rotate(-Math.PI / 4),
            Vector.Left.Rotate(-Math.PI / 5),
            Vector.Left.Rotate(-Math.PI / 6),
            Vector.Left.Rotate(-Math.PI / 7),
            Vector.Left.Rotate(-Math.PI / 8),
            Vector.Left.Rotate(-Math.PI / 9),
            Vector.Left.Rotate(-Math.PI / 10),
            Vector.Left);

        public static Func<Vector, IMovingPattern> MoveForBombing = location =>
        {
            var direction = Math.Abs(location.X) < Math.Abs(location.X - Game.GameFeatures.FieldSize.Width)
                ? Vector.Left : Vector.Right;
            return new VectorMove(
                location, 40,
                Vector.Down,
                Vector.Zero,
                Vector.Zero,
                Vector.Zero,
                direction,
                direction,
                direction,
                direction);
        };

        public static Func<Vector, IMovingPattern> DiagonalMovingLeft = location => new VectorMove(location,
            50,
            Vector.Right.Rotate(Math.PI / 3));

        public static Func<Vector, IMovingPattern> DiagonalMovingRight = location => new VectorMove(location,
            50,
            Vector.Left.Rotate(-Math.PI / 3));

        public static Func<Vector, IMovingPattern> EightLeft = location =>
            new VectorMove(
                location, 10,
                Vector.Down.Rotate(-Math.PI / 6),
                Vector.Down.Rotate(-Math.PI / 5),
                Vector.Down.Rotate(-Math.PI / 4),
                Vector.Down.Rotate(-Math.PI / 4),
                Vector.Down.Rotate(-Math.PI / 4),
                Vector.Down.Rotate(-Math.PI / 5),
                Vector.Down.Rotate(-Math.PI / 6),
                Vector.Down,
                Vector.Down.Rotate(Math.PI / 6),
                Vector.Down.Rotate(Math.PI / 5),
                Vector.Down.Rotate(Math.PI / 4),
                Vector.Down.Rotate(Math.PI / 3),
                Vector.Left,
                Vector.Left.Rotate(Math.PI / 6),
                Vector.Left.Rotate(Math.PI / 5),
                Vector.Left.Rotate(Math.PI / 4),
                Vector.Left.Rotate(Math.PI / 3),
                Vector.Up,
                Vector.Up.Rotate(Math.PI / 6),
                Vector.Up.Rotate(Math.PI / 5),
                Vector.Up.Rotate(Math.PI / 4),
                Vector.Up.Rotate(Math.PI / 5),
                Vector.Up.Rotate(Math.PI / 6),
                Vector.Up,
                Vector.Up,
                Vector.Up,
                Vector.Up);


        public static Func<Vector, IMovingPattern> EightRight = location =>
            new VectorMove(
                location, 10,
                Vector.Down.Rotate(Math.PI / 6),
                Vector.Down.Rotate(Math.PI / 5),
                Vector.Down.Rotate(Math.PI / 4),
                Vector.Down.Rotate(Math.PI / 4),
                Vector.Down.Rotate(Math.PI / 4),
                Vector.Down.Rotate(Math.PI / 5),
                Vector.Down.Rotate(Math.PI / 6),
                Vector.Down,
                Vector.Down.Rotate(-Math.PI / 6),
                Vector.Down.Rotate(-Math.PI / 5),
                Vector.Down.Rotate(-Math.PI / 4),
                Vector.Down.Rotate(-Math.PI / 3),
                Vector.Right,
                Vector.Right.Rotate(-Math.PI / 6),
                Vector.Right.Rotate(-Math.PI / 5),
                Vector.Right.Rotate(-Math.PI / 4),
                Vector.Right.Rotate(-Math.PI / 3),
                Vector.Up,
                Vector.Up.Rotate(-Math.PI / 6),
                Vector.Up.Rotate(-Math.PI / 5),
                Vector.Up.Rotate(-Math.PI / 4),
                Vector.Up.Rotate(-Math.PI / 5),
                Vector.Up.Rotate(-Math.PI / 6),
                Vector.Up,
                Vector.Up,
                Vector.Up,
                Vector.Up);

        public static Func<Vector, IMovingPattern> BossMoves = location =>
            new VectorMove(
                location,
                70,
                Vector.Zero,
                Vector.Zero,
                Vector.Up.Rotate(Math.PI / 4),
                Vector.Zero,
                Vector.Zero,
                Vector.Left.Rotate(-Math.PI / 4),
                Vector.Zero,
                Vector.Zero,
                Vector.Up.Rotate(-Math.PI / 4),
                Vector.Zero,
                Vector.Zero,
                Vector.Right.Rotate(Math.PI / 4),
                Vector.Zero,
                Vector.Zero,
                Vector.Right,
                Vector.Zero,
                Vector.Zero,
                Vector.Left,
                Vector.Zero,
                Vector.Zero,
                Vector.Left,
                Vector.Zero,
                Vector.Zero,
                Vector.Right,
                Vector.Zero,
                Vector.Zero
                );

        public static Func<Vector, Vector, IMovingPattern> FlyAndFall = (location, velocity) =>
            new VectorMove(
                location,
                70,
                velocity,
                Vector.Zero,
                Vector.Down,
                Vector.Down,
                Vector.Down,
                Vector.Down);

        public static Func<Vector, Vector, Vector,IMovingPattern> FlyAndToTarget =
            (location, velocity, target) =>
            new VectorMove(
                location,
                60,
                velocity,
                Vector.Zero,
                target,
                target,
                target,
                target);

        public static Func<Vector, Vector, IMovingPattern> MoveToTarget = (location, target) =>
            new MoveToTarget(
                location,
                target
                );
    }
}

