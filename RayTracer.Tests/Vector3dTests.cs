namespace RayTracer.Tests;

using RayTracer.Core;

[TestClass]
public sealed class Vector3dTests
{
    [TestMethod]
    public void AddSubtractAndMultiplyOperateComponentWise()
    {
        var first = new Vector3d(1, 2, 3);
        var second = new Vector3d(4, 5, 6);

        AssertVectorEqual(new Vector3d(5, 7, 9), first + second);
        AssertVectorEqual(new Vector3d(-3, -3, -3), first - second);
        AssertVectorEqual(new Vector3d(2, 4, 6), first * 2);
        AssertVectorEqual(new Vector3d(2, 4, 6), 2 * first);
        AssertVectorEqual(new Vector3d(4, 10, 18), first * second);
    }

    [TestMethod]
    public void DotAndLengthReturnExpectedValues()
    {
        var vector = new Vector3d(1, 2, 3);

        Assert.AreEqual(14f, vector.Dot(new Vector3d(1, 2, 3)));
        Assert.AreEqual(MathF.Sqrt(14), vector.Length(), 0.0001f);
    }

    [TestMethod]
    public void NormalizedReturnsUnitVector()
    {
        var normalized = new Vector3d(3, 4, 0).Normalized();

        AssertVectorEqual(new Vector3d(0.6f, 0.8f, 0), normalized);
        Assert.AreEqual(1f, normalized.Length(), 0.0001f);
    }

    [TestMethod]
    public void NormalizedRejectsZeroLengthVector()
    {
        Assert.Throws<InvalidOperationException>(
            () => new Vector3d(0, 0, 0).Normalized());
    }

    [TestMethod]
    public void ReflectReflectsAcrossSurfaceNormal()
    {
        var n = new Vector3d(0, 1, 0);
        var l = new Vector3d(-1, 1, 0).Normalized();

        var reflected = l.Reflect(n);

        AssertVectorEqual(new Vector3d(1, 1, 0).Normalized(), reflected);
    }

    [TestMethod]
    public void ToStringIncludesComponents()
    {
        Assert.AreEqual("Vector3d(1, 2, 3)", new Vector3d(1, 2, 3).ToString());
    }

    private static void AssertVectorEqual(Vector3d expected, Vector3d actual)
    {
        Assert.AreEqual(expected.X, actual.X, 0.0001f);
        Assert.AreEqual(expected.Y, actual.Y, 0.0001f);
        Assert.AreEqual(expected.Z, actual.Z, 0.0001f);
    }
}