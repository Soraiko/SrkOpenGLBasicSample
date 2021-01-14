#version 330 compatibility
uniform sampler2D texture0;

in vec2 f_texcoord;

void main()
{
	vec4 color = texture(texture0, f_texcoord);
	gl_FragColor = color;
}