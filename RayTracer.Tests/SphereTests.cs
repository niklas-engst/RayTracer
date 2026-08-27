namespace RayTracer.Tests;

using RayTracer.Core;

[TestClass]
public sealed class SphereTests
{
    private static readonly Material Material = new(new Color(100, 150, 200, 255), 0.5f, 16f);
    private static readonly Sphere Sphere = new(new Vector3d(0, 0, 0), 1f, Material);

    [TestMethod]
    public void IntersectionReturnsNearestPositiveHit()
    {
        var ray = new Ray(new Vector3d(0, 0, -3), new Vector3d(0, 0, 1));

        Assert.AreEqual(2f, Sphere.Intersection(ray), 0.0001f);
    }

    [TestMethod]
    public void IntersectionReturnsTangentHit()
    {
        var ray = new Ray(new Vector3d(0, 1, -3), new Vector3d(0, 0, 1));

        Assert.AreEqual(3f, Sphere.Intersection(ray), 0.0001f);
    }

    [TestMethod]
    public void IntersectionReturnsInfinityForMissAndBehindRay()
    {
        var miss = new Ray(new Vector3d(0, 2, -3), new Vector3d(0, 0, 1));
        var behind = new Ray(new Vector3d(0, 0, 3), new Vector3d(0, 0, 1));

        Assert.IsTrue(float.IsPositiveInfinity(Sphere.Intersection(miss)));
        Assert.IsTrue(float.IsPositiveInfinity(Sphere.Intersection(behind)));
    }

    [TestMethod]
    public void NormalAtReturnsNormalizedSurfaceNormal()
    {
        AssertVectorEqual(new Vector3d(0, 1, 0), Sphere.NormalAt(new Vector3d(0, 1, 0)));
    }

    [TestMethod]
    public void DiffuseShadingUsesLambertianIntensity()
    {
        var light = new Light(new Vector3d(0, 0, -2));

        AssertColorEqual(Material.KDiffuse, Sphere.DiffuseShading(new Vector3d(0, 0, -1), light));
    }

    [TestMethod]
    public void DiffuseShadingReturnsBlackWhenLightIsBehindSurface()
    {
        var light = new Light(new Vector3d(0, 0, 2));

        AssertColorEqual(new Color(0, 0, 0, 0), Sphere.DiffuseShading(new Vector3d(0, 0, -1), light));
    }

    private static void AssertColorEqual(Color expected, Color actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }

    private static void AssertVectorEqual(Vector3d expected, Vector3d actual)
    {
        Assert.AreEqual(expected.X, actual.X, 0.0001f);
        Assert.AreEqual(expected.Y, actual.Y, 0.0001f);
        Assert.AreEqual(expected.Z, actual.Z, 0.0001f);
    }
}
