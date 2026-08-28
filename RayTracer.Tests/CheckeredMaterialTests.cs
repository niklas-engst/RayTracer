namespace RayTracer.Tests;

using RayTracer.Core;

[TestClass]
public class CheckeredMaterialTests
{
    private static readonly Color Green = Color.FromFloat(0, 1, 0);
    private static readonly Color Magenta = Color.FromFloat(1, 0, 1);
    private static readonly CheckeredMaterial Checkered = new(Green, Magenta, 0.5f, 100f, 16, 8);

    [TestMethod]
    public void GetDiffuseColorReturnsFirstColorInEvenCheckerCell()
    {
        AssertColorEqual(Green, Checkered.GetDiffuseColor(0f, 0f));
    }

    [TestMethod]
    public void GetDiffuseColorReturnsSecondColorInOddCheckerCell()
    {
        AssertColorEqual(Magenta, Checkered.GetDiffuseColor(0.1f, 0f));
    }

    [TestMethod]
    public void SphereWithCheckeredMaterialReturnsCheckerColorAtRayHitPoint()
    {
        var sphere = new Sphere(new Vector3d(0, 0, 1), 1, Checkered);
        var ray = new Ray(new Vector3d(0, 0, -1), new Vector3d(0, 0, 1));

        var hitPoint = ray.At(sphere.Intersection(ray));
        var normal = sphere.NormalAt(hitPoint);
        var (u, v) = sphere.MakeUVs(normal);

        AssertColorEqual(Green, Checkered.GetDiffuseColor(u, v));
    }

    private static void AssertColorEqual(Color expected, Color actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }
}