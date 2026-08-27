namespace RayTracer.Tests;

using RayTracer.Core;

[TestClass]
public sealed class ColorTests
{
    [TestMethod]
    public void AdditionClampsChannelsToByteRange()
    {
        var result = new Color(200, 2, 100, 250) + new Color(100, 3, 100, 10);

        AssertColorEqual(new Color(255, 5, 200, 255), result);
    }

    [TestMethod]
    public void MultiplicationClampsAndTruncatesChannels()
    {
        AssertColorEqual(new Color(150, 1, 255, 255), new Color(100, 1, 200, 255) * 1.5f);
        AssertColorEqual(new Color(0, 0, 0, 0), new Color(100, 1, 200, 255) * -1f);
    }

    [TestMethod]
    public void ToUInt32PacksChannelsAsArgb()
    {
        Assert.AreEqual(0x04010203u, new Color(1, 2, 3, 4).ToUInt32());
    }

    private static void AssertColorEqual(Color expected, Color actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }
}
