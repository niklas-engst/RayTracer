using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RayTracer.Render;

public class ShaderProgram : IDisposable
{
    public int Handle { get; }

    public ShaderProgram(string vertexPath, string fragmentPath)
    {
        var vertexShader = CompileFromFile(
            ShaderType.VertexShader,
            vertexPath);

        var fragmentShader = CompileFromFile(
            ShaderType.FragmentShader,
            fragmentPath);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linked);

        if (linked == 0)
        {
            string log = GL.GetProgramInfoLog(Handle);

            GL.DeleteProgram(Handle);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            throw new InvalidOperationException(
                $"Shader link failed.\n{log}");
        }

        // The shader source is now linked into the program.
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void SetInt(string name, int value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }

    public void SetFloat(string name, float value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }

    public void SetVector2(string name, Vector2 value)
    {
        GL.Uniform2(GetUniformLocation(name), value);
    }

    public void SetVector3(string name, Vector3 value)
    {
        GL.Uniform3(GetUniformLocation(name), value);
    }

    public void SetMatrix4(string name, Matrix4 value)
    {
        GL.UniformMatrix4(
            GetUniformLocation(name),
            false,
            ref value);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
        GC.SuppressFinalize(this);
    }

    private static int CompileFromFile(ShaderType type, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Shader file was not found: {path}",
                path);
        }

        var source = File.ReadAllText(path);
        var shader = GL.CreateShader(type);

        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compiled);

        if (compiled == 0)
        {
            var log = GL.GetShaderInfoLog(shader);

            GL.DeleteShader(shader);

            throw new InvalidOperationException(
                $"Failed to compile {type} shader:\n" +
                $"File: {path}\n\n" +
                $"{log}");
        }

        return shader;
    }

    private int GetUniformLocation(string name)
    {
        int location = GL.GetUniformLocation(Handle, name);

        if (location == -1)
        {
            throw new InvalidOperationException(
                $"Uniform '{name}' was not found or was optimized out.");
        }

        return location;
    }
}