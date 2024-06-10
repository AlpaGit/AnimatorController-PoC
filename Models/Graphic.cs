using System.Numerics;
using System.Text.Json.Serialization;

namespace AnimatorController.Models;

public class Graphic
{
    [JsonPropertyName("atlas")]
    public FileAsset Atlas { get; set; }

    [JsonPropertyName("vertices")]
    public IList<Vector2> Vertices { get; set; }

    [JsonPropertyName("uvs")]
    public IList<Vector2> Uvs { get; set; }

    [JsonPropertyName("triangles")]
    public IList<int> Triangles { get; set; }
}