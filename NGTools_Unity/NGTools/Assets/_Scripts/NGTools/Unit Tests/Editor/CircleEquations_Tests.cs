using NUnit.Framework;

namespace UnityEngine
{
    [TestFixture]
    [Category("Extensions")]
    public class CircleEquations_Tests
    {
        #region Test Case Data
#pragma warning disable 0414
        static float[] CircumferenceRadiusConversionTests =
        {
            -1,0,1, float.MinValue, float.MaxValue
        };

        /// <summary>
        /// Only tests unit circle at origin: 1,1 at 45 degree increments.
        /// </summary>
        static object[] CirclePoints =
        {
            new object[] { 1, 0, new Vector2(2f,1f), Vector2.one},
            new object[] { 1, 45, new Vector2(1.707107f,1.707107f), Vector2.one},
            new object[] { 1, 90, new Vector2(0.9999999f,2f), Vector2.one},
            new object[] { 1, 135, new Vector2(0.2928932f,1.707107f), Vector2.one},
            new object[] { 1, 180, new Vector2(0f,0.9999999f), Vector2.one},
            new object[] { 1, 225, new Vector2(0.2928934f,0.2928931f), Vector2.one},
            new object[] { 1, 270, new Vector2(1f,0f), Vector2.one},
            new object[] { 1, 315, new Vector2(1.707107f,0.2928935f), Vector2.one}
        };
#pragma warning restore
        #endregion

        [Test, TestCaseSource("CircumferenceRadiusConversionTests")]
        public void Circumference_Radius_Tests(float value)
        {
            float circumference = Geometry.Circumference(value);
            float radius = Geometry.Radius(circumference);
            float circumferenceBack = Geometry.Circumference(radius);
            Assert.AreEqual(value, radius, "Calculated radius does not equal input radius.");
            Assert.AreEqual(circumference, circumferenceBack, "Recalculated circumference does not equal initial circumference");
        }


        [Test, TestCaseSource("CirclePoints")]
        public void PointOnCircle_Test(float radius, float angle, Vector2 expected, Vector2 center)
        {
            Vector2 actual = Geometry.PointOnCircle(radius, angle, center);
            if (actual != expected)
                Assert.Fail("Expected: {0}\nActual: {1}", expected, actual);
        }

        [Test]
        // Test inputting radians of 45 degrees (0.785398)
        public void PointOnCircle_Test_Fail()
        {
            Vector2 unexpected = new Vector2(1.707107f, 1.707107f);
            Vector2 actual = Geometry.PointOnCircle(1, 0.785398f, Vector2.one);
            if (unexpected == actual)
                Assert.Fail("Unexpected: {0}\nActual: {1}", unexpected, actual);
        }
    }
}
