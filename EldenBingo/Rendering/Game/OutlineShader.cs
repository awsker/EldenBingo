using SFML.Graphics;

namespace EldenBingo.Rendering.Game
{
    public static class OutlineShader
    {
        private static string fragmentShader = @"uniform sampler2D texture;
uniform int outlinewidth;
uniform vec4 outlinecolor;

void main()
{
    vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);
    if(pixel.a < 1f)
    {
        vec2 onePixel = vec2(1, 1) / textureSize(texture,0);
        uint w = (outlinewidth*2+1);
        vec4 c = vec4(outlinecolor.rgb, 0);
        for(int i=w*w-1;i>=0;--i)
        {
            int dx = i % w - outlinewidth;
            int dy = i / w - outlinewidth;
            float dist = length(vec2(dx, dy));
            float distFrac = max(0, (outlinewidth - dist) / outlinewidth);
            vec4 neighbour = texture2D(texture, gl_TexCoord[0].xy + vec2(onePixel.x * dx, onePixel.y * dy));
            c.a += (1f - c.a) * neighbour.a * distFrac;
        }
        c.a *= outlinecolor.a;
        pixel = pixel * (pixel.a) + c * (1 - pixel.a);
    }
    gl_FragColor = pixel;
}";

        public static Shader Create()
        {
            return Shader.FromString(null, null, fragmentShader);
        }
    }
}