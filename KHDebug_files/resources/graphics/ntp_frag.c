#version 330 compatibility
uniform sampler2D texture0;

uniform sampler2D bump_mapping;

in vec3 f_position;
in vec2 f_texcoord;
in vec3 f_normal;

uniform vec3 light0_position;
uniform vec3 light0_color;
uniform float light0_ambiant_strength;

void main()
{
    vec3 ambient = light0_ambiant_strength * light0_color;
	vec3 col = light0_color;
	vec3 normal = texture(bump_mapping, f_texcoord).rgb;
	normal = normal * 2.0 - 1.0;

	normal = normalize(normal);

	vec3 tangent = cross(f_normal, vec3(1, 0, 0));
    if (dot(tangent, tangent) < 0.01)
	{
		tangent = cross(f_normal, vec3(0, 1, 0));
	}
    tangent = normalize(tangent);
    vec3 bitangent = normalize(cross(f_normal, tangent));

	mat3 tbn = mat3(tangent, bitangent, f_normal);
	normal = normalize(tbn * normal);


	vec3 lightDir = normalize(light0_position - f_position);
	
	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = diff * col;
	vec3 amb_n_diffuse = (ambient + diffuse);

	gl_FragColor = vec4(amb_n_diffuse, 1.0) * texture(texture0, f_texcoord);
}