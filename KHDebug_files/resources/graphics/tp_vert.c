#version 330 compatibility
layout (location = 0) in vec3 v_position;
layout (location = 1) in vec2 v_texcoord;

out vec2 f_texcoord;

void main()
{
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix *  vec4(v_position, 1);
	f_texcoord = vec2(v_texcoord.x/4096.0, v_texcoord.y/4096.0);
}