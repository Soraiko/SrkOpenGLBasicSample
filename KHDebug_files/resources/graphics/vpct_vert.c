#version 330 compatibility
layout (location = 0) in vec3 position;
layout (location = 1) in vec2 v_texcoord;
layout (location = 2) in vec4 v_color;

out vec2 f_texcoord;
out vec4 f_color;

void main()
{
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix *  vec4(position, 1);
	f_texcoord = v_texcoord;
	f_color = v_color;
}