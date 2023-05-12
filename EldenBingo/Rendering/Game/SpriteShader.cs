using SFML.Graphics;

namespace EldenBingo.Rendering.Game
{
    public static class SpriteShader
    {
        private const string fragmentShader = @"uniform sampler2D texture;
uniform float alpha;
uniform vec4 tint;

void main()
{
    // lookup the pixel in the texture
    vec4 pixel = texture2D(texture, gl_TexCoord[0].xy) * (tint/255f);
    pixel.a *= alpha;

    // multiply it by the color
    gl_FragColor = gl_Color * pixel;
}";

        private const string vertexShader = @"void main()
{
    // transform the vertex position
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

    // transform the texture coordinates
    gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;

    // forward the vertex color
    gl_FrontColor = gl_Color;
}";

        public static Shader Create()
        {
            return Shader.FromString(vertexShader, null, fragmentShader);
        }
    }
}