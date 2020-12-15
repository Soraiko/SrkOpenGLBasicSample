#version 330 compatibility
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 v_texcoord;
layout (location = 3) in vec3 v_normal;

out vec2 f_texcoord;
out vec3 f_normal;

void main()
{
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(position, 1);
	f_texcoord = v_texcoord;
	f_normal = v_normal;
}