using NUnit.Framework;
using System;
using System.Drawing;

namespace Game.Tests
{
    class MovingPatterns_Tests
    {

        [Test]
        public void OneVectorMoveCorrect()
        {
            var pattern = new VectorMove(new Vector(0, 0), 2, Vector.Right);

            for (var i = 0; i < 3; i++)
                pattern.Move(1);

            Assert.AreEqual(new Vector(3, 0), pattern.Location);

        }

        [Test]
        public void TwoVectorMoveCorrect()
        {
            var pattern = new VectorMove(new Vector(0, 0), 2, Vector.Right, Vector.Down);

            for (var i = 0; i < 5; i++)
                pattern.Move(1);

            Assert.AreEqual(new Vector(3, 2), pattern.Location);
        }

        [Test]
        public void ThreeVectorMoveCorrect()
        {
            var pattern = new VectorMove(new Vector(0, 0), 2, Vector.Right, Vector.Down, Vector.Left);

            for (var i = 0; i < 7; i++)
                pattern.Move(1);

            Assert.AreEqual(new Vector(1, 2), pattern.Location);
        }

        [Test]
        public void AngleVectorMoveCorrect()
        {
            var pattern = new VectorMove(new Vector(0, 0), 2, Vector.Right.Rotate(Math.PI / 3));

            for (var i = 0; i < 2; i++)
                pattern.Move(1);

            Assert.AreEqual(1, pattern.Location.X, 1e-9);
            Assert.AreEqual(2 * Math.Sin(Math.PI / 3), pattern.Location.Y, 1e-9);
        }
    }
}
