#version 330 compatibility
layout (location = 0) in vec3 v_position;
layout (location = 1) in vec2 v_texcoord;
layout (location = 2) in vec3 v_normal;
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

layout(std140) uniform transform_data
{
  mat4 matrices[1024];
};


void main()
{
	vec4 position_v4 = vec4(v_position, 1);
	vec4 influenced_position = vec4(0, 0, 0, 0);

	vec4 influenced_normal = vec4(0, 0, 0, 0);
	vec4 transformed_normal_v4 = vec4(v_position + v_normal * 100.0, 1);

	if (uint(v_infCount)>uint(0))
	{
		uint inf0_index = (uint(inf0) & uint(65535));
		float inf0_weighting = float((uint(inf0) & uint(4294901760)) >> 16) / 65535.0;

		mat4 inf0_matrix = matrices[uint(512) + inf0_index] * matrices[inf0_index] * inf0_weighting;
		influenced_position += position_v4 * inf0_matrix;
		influenced_normal += transformed_normal_v4 * inf0_matrix;
		
		if (uint(v_infCount)>uint(1))
		{
			uint inf1_index = (uint(inf1) & uint(65535));
			float inf1_weighting = float((uint(inf1) & uint(4294901760)) >> 16) / 65535.0;
			
			mat4 inf1_matrix = matrices[uint(512) + inf1_index] * matrices[inf1_index] * inf1_weighting;
			influenced_position += position_v4 * inf1_matrix;
			influenced_normal += transformed_normal_v4 * inf1_matrix;

			if (uint(v_infCount)>uint(2))
			{
				uint inf2_index = (uint(inf2) & uint(65535));
				float inf2_weighting = float((uint(inf2) & uint(4294901760)) >> 16) / 65535.0;
			
				mat4 inf2_matrix = matrices[uint(512) + inf2_index] * matrices[inf2_index] * inf2_weighting;
				influenced_position += position_v4 * inf2_matrix;
				influenced_normal += transformed_normal_v4 * inf2_matrix;

				if (uint(v_infCount)>uint(3))
				{
					uint inf3_index = (uint(inf3) & uint(65535));
					float inf3_weighting = float((uint(inf3) & uint(4294901760)) >> 16) / 65535.0;
			
					mat4 inf3_matrix = matrices[uint(512) + inf3_index] * matrices[inf3_index] * inf3_weighting;
					influenced_position += position_v4 * inf3_matrix;
					influenced_normal += transformed_normal_v4 * inf3_matrix;

					if (uint(v_infCount)>uint(4))
					{
						uint inf4_index = (uint(inf4) & uint(65535));
						float inf4_weighting = float((uint(inf4) & uint(4294901760)) >> 16) / 65535.0;
						
						mat4 inf4_matrix = matrices[uint(512) + inf4_index] * matrices[inf4_index] * inf4_weighting;
						influenced_position += position_v4 * inf4_matrix;
						influenced_normal += transformed_normal_v4 * inf4_matrix;

						if (uint(v_infCount)>uint(5))
						{
							uint inf5_index = (uint(inf5) & uint(65535));
							float inf5_weighting = float((uint(inf5) & uint(4294901760)) >> 16) / 65535.0;
							
							mat4 inf5_matrix = matrices[uint(512) + inf5_index] * matrices[inf5_index] * inf5_weighting;
							influenced_position += position_v4 * inf5_matrix;
							influenced_normal += transformed_normal_v4 * inf5_matrix;

							if (uint(v_infCount)>uint(6))
							{
								uint inf6_index = (uint(inf6) & uint(65535));
								float inf6_weighting = float((uint(inf6) & uint(4294901760)) >> 16) / 65535.0;
								
								mat4 inf6_matrix = matrices[uint(512) + inf6_index] * matrices[inf6_index] * inf6_weighting;
								influenced_position += position_v4 * inf6_matrix;
								influenced_normal += transformed_normal_v4 * inf6_matrix;
							}
						}
					}
				}
			}
		}
	}
	
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix *  influenced_position;
	f_position = influenced_position.xyz;
	f_texcoord = v_texcoord/4096.0;
	f_normal = (influenced_normal.xyz - influenced_position.xyz)/100.0;
}