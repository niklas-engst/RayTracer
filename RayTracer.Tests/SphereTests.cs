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
        var normal = new Vector3d(0, 0, -1);
        var lightDirection = new Vector3d(0, 0, -1);

        AssertColorEqual(Material.KDiffuse, Sphere.DiffuseShading(normal, lightDirection));
    }

    [TestMethod]
    public void DiffuseShadingReturnsBlackWhenLightIsBehindSurface()
    {
        var normal = new Vector3d(0, 0, -1);
        var lightDirection = new Vector3d(0, 0, 1);

        AssertColorEqual(new Color(0, 0, 0, 0), Sphere.DiffuseShading(normal, lightDirection));
    }

    [TestMethod]
    public void SpecularShadingProducesHighlightWhenReflectionPointsTowardLightSource()
    {
        var normal = new Vector3d(0, 0, 1);
        var lightDirection = new Vector3d(0, 0, 1);
        var viewDirection = new Vector3d(0, 0, -1);

        var result = Sphere.SpecularShading(normal, lightDirection, viewDirection);

        AssertColorEqual(Color.FromFloat(0.5f, 0.5f, 0.5f), result);
    }

    [TestMethod]
    public void SpecularShadingReturnsBlackWhenViewMissesReflection()
    {
        var normal = new Vector3d(0, 0, 1);
        var lightDirection = new Vector3d(0, 0, 1);
        var viewDirection = new Vector3d(1, 0, 0);

        var result = Sphere.SpecularShading(normal, lightDirection, viewDirection);

        AssertColorEqual(new Color(0, 0, 0, 255), result);
    }
    
    [TestMethod]
    public void MakeUvsReturnsCorrectUvCoordinates()
    {
        var sphere = new Sphere(new Vector3d(0, 0, 1), 1, new Material(new Color(0, 0, 0, 255), 1, 1));
        var ray = new Ray(new Vector3d(0, 0, -1), new Vector3d(0, 0, 1));
        var uvs = sphere.MakeUVs(sphere.NormalAt(ray.At(sphere.Intersection(ray))));
        
        Assert.HasCount(2, uvs);
        
        var expectedUvs = new[] { -0.25f, 0.5f };
        CollectionAssert.AreEqual(expectedUvs, uvs);
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
