using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using AnimatorController.Models;

namespace AnimatorController.Readers;

public class AnimationRenderer
{
    public int CurrentFrame { get; private set; }
    public RenderState[] RenderStates { get; private set; }
    
    private static readonly List<Vector3> VertexBuffer = new();
    private static readonly List<Vector2> UvBuffer = new();
    private static readonly List<Vector3> ColorBuffer = new();
    private static readonly List<int> TriangleBuffer = new();

    public AnimationRenderer(int maxNodeCount)
    {
        RenderStates = new RenderState[maxNodeCount];
    }

    public unsafe void Render(AnimationAtlas atlas, AnimationMetadata metadata, int frame)
    {
        if (frame == CurrentFrame)
        {
            return;
        }

        var curFrame = CurrentFrame;
        
        var nodesCount = metadata.NodeCount;

        if (frame < CurrentFrame)
        {
            curFrame = -1;
            for (var i = 0; i < nodesCount; ++i)
            {
                RenderStates[i].Reset();
            }
        }
        
        var bytes = metadata.Data;
        int frameDataPosition;
        
        if (curFrame == frame - 1)
        {
            frameDataPosition = metadata.FrameDataPositions[frame];
        }
        else
        {
            frameDataPosition = metadata.FrameDataPositions[curFrame + 1];

            for (; curFrame < frame - 1; ++curFrame)
            {
                for (var i = nodesCount - 1; i >= 0; --i)
                {
                    int num;
            
                    fixed (byte* p = &bytes[frameDataPosition])
                    {
                        num = *(short*)p;
                    }
                    
                    var dataPosition = frameDataPosition + 2;
                    frameDataPosition = RenderStates[num].Compute(bytes, dataPosition);
                }
            }
        }


        var num2 = 0;
        for (var i = nodesCount - 1; i >= 0; --i)
        {
            int num;
            
            fixed (byte* p = &bytes[frameDataPosition])
            {
                num = *(short*)p;
            }
            
            var dataPosition = frameDataPosition + 2;
            frameDataPosition = RenderStates[num].Compute(bytes, dataPosition);

            var renderState = RenderStates[num];

            if (renderState.spriteIndex < 0) 
                continue;
            
            var graphic = atlas.GraphicDatas[renderState.spriteIndex];

            var m00 = renderState.m00;
            var m01 = renderState.m01;
            var m03 = renderState.m03;
            var m10 = renderState.m10;
            var m11 = renderState.m11;
            var m13 = renderState.m13;

            var vertexBuffer = VertexBuffer;
            var colorBuffer = ColorBuffer;
            var uvBuffer = UvBuffer;
            var triangleBuffer = TriangleBuffer;
                
                
            var colors = new Vector3((float) renderState.alpha / byte.MaxValue, renderState.multiplicativeColor, renderState.additiveColor);
            var vertices = graphic.Vertices;
            var verticesLength = vertices.Count;

            for(var j = 0; j < verticesLength; ++j)
            {
                var vector2 = vertices[j];
                Vector3 vertex;
                vertex.X = (float) (m00 * (double) vector2.X + m01 * (double) vector2.Y) + m03;
                vertex.Y = (float) (m10 * (double) vector2.X + m11 * (double) vector2.Y) + m13;
                vertex.Z = 0.0f;
                vertexBuffer.Add(vertex);
                colorBuffer.Add(colors);
            }

            var uvs = graphic.Uvs;
            var uvsLength = uvs.Count;
            for (var j = 0; j < uvsLength; ++j)
            {
                uvBuffer.Add(uvs[j]);
            }
                
            var triangles = graphic.Triangles;
            var trianglesLength = triangles.Count;
                
            for (var j = 0; j < trianglesLength; ++j)
            {
                triangleBuffer.Add(triangles[j] + num2);
            }
                
            num2 += verticesLength;
            GenerateMeshPreview(atlas, $"frame-{frame}.png");
        }
    }
    
    #pragma warning disable CA1416
    public void GenerateMeshPreview(AnimationAtlas atlas, string outputPath, int width = 800, int height = 600)
    {
        var bitmap = new Bitmap(width, height);
        var g = Graphics.FromImage(bitmap);
        g.Clear(Color.Black);

        // Scale and center the mesh in the bitmap
        float scale = 1.0f;
        var offsetX = width / 2.0f;
        var offsetY = height / 2.0f;

        // Draw triangles
        for (int i = 0; i < TriangleBuffer.Count; i += 3)
        {
            var p1 = VertexBuffer[TriangleBuffer[i]];
            var p2 = VertexBuffer[TriangleBuffer[i + 1]];
            var p3 = VertexBuffer[TriangleBuffer[i + 2]];

            PointF[] points = {
                new PointF(p1.X * scale + offsetX, p1.Y * scale + offsetY),
                new PointF(p2.X * scale + offsetX, p2.Y * scale + offsetY),
                new PointF(p3.X * scale + offsetX, p3.Y * scale + offsetY)
            };

            g.FillPolygon(Brushes.White, points);
            g.DrawPolygon(Pens.Red, points);
        }

        // Draw vertices
        foreach (var vertex in VertexBuffer)
        {
            var x = vertex.X * scale + offsetX;
            var y = vertex.Y * scale + offsetY;
            g.FillEllipse(Brushes.Blue, x - 2, y - 2, 4, 4);
        }

        bitmap.Save(outputPath, ImageFormat.Png);
    }
}