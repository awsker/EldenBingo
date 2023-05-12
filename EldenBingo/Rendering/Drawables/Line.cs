using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Drawables
{
    public class Line : Transformable, IDrawable
    {
        private const string vertexShader = @"
uniform float linewidth;
void main()
{
    vec4 delta = vec4(gl_MultiTexCoord0.st * linewidth * 0.5, 0, 0);
    vec4 position = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * (position + delta);

    gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
    gl_TexCoord[1] = vec4(1, 0, 0, 0);

    gl_FrontColor = gl_Color;
}";

        private const string fragmentShader = @"
uniform vec4 tint;
void main()
{
    vec4 color = gl_Color; 
    color.a *= min(1f, (1f - length(gl_TexCoord[0].xy)) * 5);
    gl_FragColor = color * tint;
}";

        private static Shader _shader;
        private IList<Vector2f> _points;
        private VertexBuffer? _buffer;
        private SFML.Graphics.Glsl.Vec4 _color;

        private float _width = 5f;
        private bool _changed = false;

        static Line()
        {
            _shader = Shader.FromString(vertexShader, null, fragmentShader);
        }

        public Line(System.Drawing.Color color, Vector2f start)
        {
            _color = new SFML.Graphics.Glsl.Vec4(new SFML.Graphics.Color(color.R, color.G, color.B, 255));
            _points = new List<Vector2f>() { start };
        }

        public Line(System.Drawing.Color color, Vector2f start, Vector2f end)
        {
            _color = new SFML.Graphics.Glsl.Vec4(new SFML.Graphics.Color(color.R, color.G, color.B, 255));
            _points = new List<Vector2f>() { start, end };
        }

        public bool Visible { get; set; } = true;
        public bool RoundedEnds { get; set; } = true;

        public float Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    _changed = true;
                }
            }
        }

        public void AddPoint(Vector2f p)
        {
            if (_points[_points.Count - 1].DistanceTo(p) < 4)
                return;
            _points.Add(p);
            _changed = true;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var v = target.GetView();
            if (_buffer == null || _changed)
                generateVertexBuffers();
            states.Shader = _shader;
            _shader.SetUniform("linewidth", Width);
            _shader.SetUniform("tint", _color);
            target.Draw(_buffer, states);
        }

        private void generateVertexBuffers()
        {
            var vertices = new List<Vertex>();
            if (_points.Count == 1)
            {
                if (RoundedEnds)
                {
                    var p = _points[0];
                    var normal = new Vector2f(1f, 0);
                    vertices.AddRange(createFan(p, -normal, normal, 6));
                    vertices.AddRange(createFan(p, normal, -normal, 6));
                }
            }
            else for (int i = 0; i < _points.Count - 1; ++i)
                {
                    var p = _points[i];
                    var p2 = _points[i + 1];
                    var thisVec = p2 - p;
                    Vector2f normal = thisVec.Normal();

                    if (RoundedEnds && i == 0)
                    {
                        vertices.AddRange(createFan(p, -normal, normal, 6));
                    }
                    vertices.Add(new Vertex(p, SFML.Graphics.Color.White, normal));
                    vertices.Add(new Vertex(p, SFML.Graphics.Color.White, -normal));
                    vertices.Add(new Vertex(p2, SFML.Graphics.Color.White, normal));
                    vertices.Add(new Vertex(p2, SFML.Graphics.Color.White, -normal));
                    if (i < _points.Count - 2)
                    {
                        var nextVec = _points[i + 2] - p2;
                        var nextNormal = nextVec.Normal();

                        var parts = (int)(thisVec.Angle(nextVec) / 0.52359877f);
                        var cross = thisVec.Cross(nextVec);
                        if (parts > 0)
                        {
                            if (cross < 0)
                            {
                                vertices.AddRange(createFan(p2, normal, nextNormal, parts));
                            }
                            else
                            {
                                vertices.AddRange(createFan(p2, -normal, -nextNormal, parts));
                            }
                        }
                    }
                    if (RoundedEnds && i == _points.Count - 2)
                    {
                        vertices.AddRange(createFan(p2, normal, -normal, 6));
                    }
                }
            if (_buffer == null)
                _buffer = new VertexBuffer((uint)_points.Count * 2, PrimitiveType.TriangleStrip, VertexBuffer.UsageSpecifier.Dynamic);

            _buffer.Update(vertices.ToArray());

            _changed = false;
        }

        public new void Dispose()
        {
            _buffer?.Dispose();
        }

        private IList<Vertex> createFan(Vector2f pos, Vector2f normal1, Vector2f normal2, int parts)
        {
            var vertices = new List<Vertex>();
            var angle = normal1.Angle(normal2) * (normal1.Cross(normal2) > 0 ? 1f : -1f);
            var deltaAngle = angle / parts;
            var currentAngle = 0f;

            for (int i = 0; i <= parts; i++)
            {
                vertices.Add(new Vertex(pos, SFML.Graphics.Color.White, new Vector2f()));
                vertices.Add(new Vertex(pos, SFML.Graphics.Color.White, normal1.Rotate(currentAngle)));
                currentAngle += deltaAngle;
            }
            return vertices;
        }
    }
}