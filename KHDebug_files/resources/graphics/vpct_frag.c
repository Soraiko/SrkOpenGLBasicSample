#version 330 compatibility
in vec2 f_texcoord;
uniform sampler2D texture0;
in vec4 f_color;
uniform int skipAlpha = 0;

void main()
{
	vec4 color = f_color * texture(texture0, f_texcoord);
	if (skipAlpha == 1 && color.a < 1)
		discard;
	if (skipAlpha == 0 && color.a < 0.1)
		discard;

	gl_FragColor = color;
}