#version 410 core

const vec2 Positions[3] = vec2[](
    vec2(-1.0, -1.0),
    vec2( 3.0, -1.0),
    vec2(-1.0,  3.0)
);

out vec2 vUv;

void main()
{
    vec2 position = Positions[gl_VertexID];

    gl_Position = vec4(position, 0.0, 1.0);
    vUv = vec2(position.x * 0.5 + 0.5, 1.0 - (position.y * 0.5 + 0.5)); 
}