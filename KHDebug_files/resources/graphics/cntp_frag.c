#version 330 compatibility
uniform sampler2D texture0;

uniform sampler2D bump_mapping;

in vec3 f_position;  
in vec2 f_texcoord;
in vec3 f_normal;
in vec4 f_color;

uniform vec3 light0_position;
uniform vec3 light0_color;
uniform float light0_ambiant_strength;

void main()
{
    vec3 ambient = light0_ambiant_strength * light0_color;
	
	vec3 normal = texture(bump_mapping, f_texcoord).rgb;
	normal = normal * 2.0 - 1.0;
	normal = normalize(normal);

	vec3 tangent0 = cross(f_normal, vec3(1, 0, 0));
    if (dot(tangent0, tangent0) < 0.001)
        tangent0 = cross(f_normal, vec3(0, 1, 0));
    tangent0 = normalize(tangent0);
    vec3 tangent1 = normalize(cross(f_normal, tangent0));

	mat3 tbn = mat3(tangent0, tangent1, f_normal);
	normal = normalize(tbn * normal);


	vec3 lightDir = normalize(light0_position - f_position);
	
	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = diff * light0_color;

	vec3 amb_n_diffuse = (ambient + diffuse);


	vec4 color = f_color * texture(texture0, f_texcoord);

	gl_FragColor = vec4(amb_n_diffuse, 1.0) * color;
}