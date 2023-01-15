﻿#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform sampler2D diffuse;

void main()
{
	FragColor = texture(diffuse, TexCoord);
}