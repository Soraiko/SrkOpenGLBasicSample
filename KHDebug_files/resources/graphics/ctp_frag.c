#version 330 compatibility
uniform sampler2D texture0;

in vec2 f_texcoord;
in vec4 f_color;

void main()
{
	vec4 color = f_color * texture(texture0, f_texcoord);
	gl_FragColor = color;
}