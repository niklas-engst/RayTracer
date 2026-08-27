#version 410 core

in vec2 vUv;
out vec4 FragColor;

uniform sampler2D uPixels;

void main()
{
    FragColor = texture(uPixels, vUv);
}