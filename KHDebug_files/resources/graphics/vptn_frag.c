#version 330 compatibility
in vec2 f_texcoord;
uniform sampler2D texture0;
in vec3 f_normal;

void main()
{
	gl_FragColor = texture(texture0, f_texcoord);
}