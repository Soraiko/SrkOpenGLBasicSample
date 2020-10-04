#version 330 compatibility
layout (location = 0) in vec3 aPosition;

void main()
{
    gl_Position =  gl_ProjectionMatrix * gl_ModelViewMatrix * vec4(aPosition, 1);
}