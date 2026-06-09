using SFML.Graphics;

namespace EldenBingo.Rendering.Game
{
    public static class OutlineShader
    {
        private static string fragmentShader = @"#version 130
uniform sampler2D tex;
uniform int outlinewidth;
uniform vec4 outlinecolor;

void main()
{
    vec4 pixel = texture(tex, gl_TexCoord[0].xy);
    if(pixel.a < 1.0)
    {
        vec2 onePixel = vec2(1.0, 1.0) / textureSize(tex, 0);
        int w = outlinewidth * 2 + 1;
        vec4 c = vec4(outlinecolor.rgb, 0.0);
        for(int i = w * w - 1; i >= 0; --i)
        {
            int dx = i % w - outlinewidth;
            int dy = i / w - outlinewidth;
            float dist = length(vec2(float(dx), float(dy)));
            float distFrac = max(0.0, (float(outlinewidth) - dist) / outlinewidth);
            vec4 neighbour = texture(tex, gl_TexCoord[0].xy + vec2(onePixel.x * dx, onePixel.y * dy));
            c.a += (1.0 - c.a) * neighbour.a * distFrac;
        }
        c.a *= outlinecolor.a;
        pixel = pixel * (pixel.a) + c * (1.0 - pixel.a);
    }
    gl_FragColor = pixel;
}";

        public static Shader Create()
        {
            try
            {
                return Shader.FromString(null, null, fragmentShader);
            } 
            catch (Exception ex)
            {
                throw new Exception("Error creating outline shader: " + ex.Message);
            }
        }
    }
}