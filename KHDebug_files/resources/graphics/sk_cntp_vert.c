#version 330 compatibility
layout (location = 0) in vec3 v_position;
layout (location = 1) in vec2 v_texcoord;
layout (location = 2) in vec3 v_normal;
layout (location = 3) in vec4 v_color;
layout (location = 8) in uint v_infCount;
layout (location = 9) in uint inf0;
layout (location = 10) in uint inf1;
layout (location = 11) in uint inf2;
layout (location = 12) in uint inf3;
layout (location = 13) in uint inf4;
layout (location = 14) in uint inf5;
layout (location = 15) in uint inf6;

out vec3 f_position;
out vec2 f_texcoord;
out vec3 f_normal;
out vec4 f_color;

layout(std140) uniform transform_data
{
  mat4 matrices[1024];
};


void main()
{
	vec4 pos = vec4(0,0,0, 0);

	if (uint(v_infCount)>uint(0))
	{
		uint inf0_index = (uint(inf0) & uint(65535));
		float inf0_weighting = ((uint(inf0) & uint(4294901760)) >> 16) / 65535.0;

		vec4 reverse = vec4(v_position, 1) * matrices[uint(512) + inf0_index];
		reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf0_index];
		pos += reverse * inf0_weighting;


		if (uint(v_infCount)>uint(1))
		{
			uint inf1_index = (uint(inf1) & uint(65535));
			float inf1_weighting = ((uint(inf1) & uint(4294901760)) >> 16) / 65535.0;

			reverse = vec4(v_position, 1) * matrices[uint(512) + inf1_index];
			reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf1_index];
			pos += reverse * inf1_weighting;

			if (uint(v_infCount)>uint(2))
			{
				uint inf2_index = (uint(inf2) & uint(65535));
				float inf2_weighting = ((uint(inf2) & uint(4294901760)) >> 16) / 65535.0;
			
				reverse = vec4(v_position, 1) * matrices[uint(512) + inf2_index];
				reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf2_index];
				pos += reverse * inf2_weighting;

				if (uint(v_infCount)>uint(3))
				{
					uint inf3_index = (uint(inf3) & uint(65535));
					float inf3_weighting = ((uint(inf3) & uint(4294901760)) >> 16) / 65535.0;
			
					reverse = vec4(v_position, 1) * matrices[uint(512) + inf3_index];
					reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf3_index];
					pos += reverse * inf3_weighting;

					if (uint(v_infCount)>uint(4))
					{
						uint inf4_index = (uint(inf4) & uint(65535));
						float inf4_weighting = ((uint(inf4) & uint(4294901760)) >> 16) / 65535.0;

						
						reverse = vec4(v_position, 1) * matrices[uint(512) + inf4_index];
						reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf4_index];
						pos += reverse * inf4_weighting;

						if (uint(v_infCount)>uint(5))
						{
							uint inf5_index = (uint(inf5) & uint(65535));
							float inf5_weighting = ((uint(inf5) & uint(4294901760)) >> 16) / 65535.0;

							reverse = vec4(v_position, 1) * matrices[uint(512) + inf5_index];
							reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf5_index];
							pos += reverse * inf5_weighting;

							if (uint(v_infCount)>uint(6))
							{
								uint inf6_index = (uint(inf6) & uint(65535));
								float inf6_weighting = ((uint(inf6) & uint(4294901760)) >> 16) / 65535.0;

								reverse = vec4(v_position, 1) * matrices[uint(512) + inf6_index];
								reverse = vec4(reverse.x,reverse.y,reverse.z, 1) * matrices[inf6_index];
								pos += reverse * inf6_weighting;
							}
						}
					}
				}
			}
		}
	}

    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix *  pos;
	f_position = vec3(pos.x,pos.y,pos.z);
	f_texcoord = vec2(v_texcoord.x/4096.0, v_texcoord.y/4096.0);
	f_color = v_color;
}