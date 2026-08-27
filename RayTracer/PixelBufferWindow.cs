using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.OpenGL;

namespace RayTracer;

public class PixelBufferWindow(int width, int height) : GlfwWindow(width, height)
{
    protected int RenderWidth { get; private set; }
    protected int RenderHeight { get; private set; }
    protected uint[] Pixels { get; private set; } = [];

    private uint _pixelTexture;

    protected override void OnLoad()
    {
        RenderWidth = Math.Max(500, 1);
        RenderHeight = Math.Max(500, 1);
        Pixels = new uint[RenderWidth * RenderHeight];

        _pixelTexture = GL.GenTexture();
        GL.BindTexture(GLTextureTarget.Texture2D, _pixelTexture);
        GL.TexParameteri(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.MinFilter,
            (int)GLTextureMinFilter.Nearest);
        GL.TexParameteri(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.MagFilter,
            (int)GLTextureMagFilter.Nearest);
        GL.TexParameteri(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.WrapS,
            (int)GLTextureWrapMode.ClampToEdge);
        GL.TexParameteri(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.WrapT,
            (int)GLTextureWrapMode.ClampToEdge);
        GL.TexImage2D(
            GLTextureTarget.Texture2D,
            0,
            GLInternalFormat.Rgba8,
            RenderWidth,
            RenderHeight,
            0,
            GLPixelFormat.Bgra,
            GLPixelType.UnsignedByte,
            IntPtr.Zero);
        GL.BindTexture(GLTextureTarget.Texture2D, 0);
    }

    protected override void RenderInterface()
    {
        RenderPixels();

        if (!ImGui.Begin("Pixel buffer"))
        {
            ImGui.End();
            return;
        }

        RenderPixelBuffer();
        ImGui.End();
    }

    protected unsafe void RenderPixelBuffer()
    {
        UploadPixels();

        var texture = new ImTextureRef(null, (nint)_pixelTexture);
        ImGui.Image(
            texture,
            new Vector2(RenderWidth, RenderHeight),
            new Vector2(0, 0),
            new Vector2(1, 1));
    }

    protected override void OnUnload()
    {
        if (_pixelTexture != 0)
        {
            GL.DeleteTexture(_pixelTexture);
            _pixelTexture = 0;
        }
    }

    protected virtual void RenderPixels()
    {
    }

    private unsafe void UploadPixels()
    {
        GL.BindTexture(GLTextureTarget.Texture2D, _pixelTexture);
        fixed (uint* pixelPointer = Pixels)
        {
            GL.TexSubImage2D(
                GLTextureTarget.Texture2D,
                0,
                0,
                0,
                RenderWidth,
                RenderHeight,
                GLPixelFormat.Bgra,
                GLPixelType.UnsignedByte,
                pixelPointer);
        }

        GL.BindTexture(GLTextureTarget.Texture2D, 0);
    }
}