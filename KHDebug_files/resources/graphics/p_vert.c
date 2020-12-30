#version 330 compatibility
layout (location = 0) in vec3 v_position;

void main()
{
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix *  vec4(v_position, 1);
}