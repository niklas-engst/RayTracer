using Hexa.NET.ImGui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RayTracer.Render;

public class PixelBufferWindow : ImGuiWindow
{
    protected int RenderWidth { get; private set; }
    protected int RenderHeight { get; private set; }

    // One packed RGBA8 pixel per entry.
    // Write pixels as: 0xAABBGGRR.
    protected uint[] Pixels { get; private set; } = Array.Empty<uint>();

    private int _pixelTexture;
    private int _presentProgram;
    private int _fullscreenVao;

    private double _elapsedSeconds;

    public PixelBufferWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) : base(gameSettings, nativeSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        _fullscreenVao = GL.GenVertexArray();

        _presentProgram = new ShaderProgram(
            Path.Combine(AppContext.BaseDirectory, "Render", "Shaders", "fullscreen.vert"),
            Path.Combine(AppContext.BaseDirectory, "Render", "Shaders", "present.frag")
        ).Handle;

        _pixelTexture = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, _pixelTexture);

        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);

        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);

        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            (int)TextureWrapMode.ClampToEdge);

        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.ClampToEdge);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        // Allocate using the current initial window size.
        ResizePixelBuffer(Size.X, Size.Y);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        ResizePixelBuffer(e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        _elapsedSeconds += args.Time;
    }

    protected override void RenderScene(FrameEventArgs args)
    {
        RenderPixels();

        GL.Clear(ClearBufferMask.ColorBufferBit);

        UploadPixelsToGpu();
        PresentPixelTexture();
    }

    protected override void OnUnload()
    {
        GL.DeleteTexture(_pixelTexture);
        GL.DeleteVertexArray(_fullscreenVao);
        GL.DeleteProgram(_presentProgram);

        base.OnUnload();
    }
    
    protected virtual void RenderPixels() { }

    private void ResizePixelBuffer(int width, int height)
    {
        width = Math.Max(width, 1);
        height = Math.Max(height, 1);

        RenderWidth = width;
        RenderHeight = height;

        Pixels = new uint[width * height];

        GL.BindTexture(TextureTarget.Texture2D, _pixelTexture);

        // Allocate/reallocate the GPU texture. No pixel data is uploaded here.
        GL.TexImage2D(
            TextureTarget.Texture2D,
            level: 0,
            internalformat: PixelInternalFormat.Rgba8,
            width: width,
            height: height,
            border: 0,
            format: PixelFormat.Bgra,
            type: PixelType.UnsignedByte,
            pixels: IntPtr.Zero);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private unsafe void UploadPixelsToGpu()
    {
        GL.BindTexture(TextureTarget.Texture2D, _pixelTexture);

        fixed (uint* pixelPointer = Pixels)
        {
            GL.TexSubImage2D(
                TextureTarget.Texture2D,
                level: 0,
                xoffset: 0,
                yoffset: 0,
                width: RenderWidth,
                height: RenderHeight,
                format: PixelFormat.Bgra,
                type: PixelType.UnsignedByte,
                pixels: (IntPtr)pixelPointer);
        }

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void PresentPixelTexture()
    {
        GL.Disable(EnableCap.DepthTest);

        GL.UseProgram(_presentProgram);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _pixelTexture);

        var textureLocation = GL.GetUniformLocation(
            _presentProgram,
            "uPixels");

        GL.Uniform1(textureLocation, 0);

        GL.BindVertexArray(_fullscreenVao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.UseProgram(0);
    }
}