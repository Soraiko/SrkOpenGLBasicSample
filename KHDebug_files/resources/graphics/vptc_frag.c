#version 330 compatibility
in vec2 f_texcoord;
uniform sampler2D texture0;
in vec4 f_color;

void main()
{
	vec4 color = f_color * texture(texture0, f_texcoord);
	gl_FragColor = color;
}