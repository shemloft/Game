using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector(Point point) : this(point.X, point.Y)
        {
        }

        public readonly double X;
        public readonly double Y;
        public double Length => Math.Sqrt(X * X + Y * Y);
        public double Angle => Math.Atan2(Y, X);
        public static Vector Zero = new Vector(0, 0);

        public static Vector Left = new Vector(-1, 0);
        public static Vector Right = new Vector(1, 0);
        public static Vector Up = new Vector(0, -1);
        public static Vector Down = new Vector(0, 1);

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        protected bool Equals(Vector other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, double k)
        {
            return new Vector(a.X * k, a.Y * k);
        }

        public static Vector operator /(Vector a, double k)
        {
            return new Vector(a.X / k, a.Y / k);
        }

        public static Vector operator *(double k, Vector a)
        {
            return a * k;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public Vector Normalize()
        {
            return Length > 0 ? this * (1 / Length) : this;
        }

        public Vector Rotate(double angle)
        {
            return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
        }

        public Vector BoundTo(Size size) //сложно
        {
            return new Vector(Math.Max(0, Math.Min(size.Width, X)), Math.Max(0, Math.Min(size.Height, Y)));
        }

        public bool EqualWithInfelicity(Vector other, double infelicity)
        {
            return Math.Abs(X - other.X) < infelicity &&
                Math.Abs(Y - other.Y) < infelicity;
        }

        public static List<Vector> SplitVector(Vector start, Vector end)
        {
            var vector = end - start;
            var length = vector.Length;
            var vectors = new List<Vector>();
            if (length <= 1) return new List<Vector> { end };
            var normalVector = vector.Normalize();
            var count = Math.Floor(length);
            for (var i = 1; i <= count; i++)
            {
                vectors.Add(start + normalVector * i);
            }
            if (count != length) vectors.Add(end);
            
            return vectors;
        }
    }
}
