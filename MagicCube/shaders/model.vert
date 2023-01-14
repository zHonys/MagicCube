#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	FragPos  = vec3(View * Model * vec4(aPos, 1));
	Normal   = mat3(transpose(inverse(View * Model))) * aNormal;
	TexCoord = aTexCoord;
	gl_Position = Projection * View * Model * vec4(aPos, 1.0);
}