namespace RayTracer.Tests;

using RayTracer.Core;

[TestClass]
public sealed class RayTests
{
    [TestMethod]
    public void AtReturnsPointAlongRay()
    {
        var ray = new Ray(new Vector3d(1, 2, 3), new Vector3d(2, 4, 6));

        AssertVectorEqual(new Vector3d(5, 10, 15), ray.At(2));
    }

    private static void AssertVectorEqual(Vector3d expected, Vector3d actual)
    {
        Assert.AreEqual(expected.X, actual.X, 0.0001f);
        Assert.AreEqual(expected.Y, actual.Y, 0.0001f);
        Assert.AreEqual(expected.Z, actual.Z, 0.0001f);
    }
}
