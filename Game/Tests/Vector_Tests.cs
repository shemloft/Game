using NUnit.Framework;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace Game.Tests
{
    class Vector_Tests
    {
        [Test]
        public void VectorLessThanOne()
        {
            VectorSplitTest(new Vector(0, 0), new Vector(0, 0.5), new List<Vector> {
            new Vector(0, 0.5)});
        }

        [Test]
        public void VectorWithNonIntegerLength()
        {
            VectorSplitTest(new Vector(0, 0), new Vector(0, 2.5), new List<Vector> {
                new Vector(0, 1),
                new Vector(0, 2),
                new Vector(0, 2.5)});
        }

        [Test]
        public void VectorWithIntegerLength()
        {
            VectorSplitTest(new Vector(0, 0), new Vector(0, 3), new List<Vector> {
                new Vector(0, 1),
                new Vector(0, 2),
                new Vector(0, 3)});
        }


        public void VectorSplitTest(Vector start, Vector end, List<Vector> expectedResult)
        {
            var result = Vector.SplitVector(start, end);
            CollectionAssert.AreEqual(expectedResult, result);
        }
    }
}
