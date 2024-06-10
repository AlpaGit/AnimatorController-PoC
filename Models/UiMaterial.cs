using System.Text.Json.Serialization;

namespace AnimatorController.Models;

public class UiMaterial
{
    [JsonPropertyName("m_FileID")]
    public int FileId { get; set; }

    [JsonPropertyName("m_PathID")]
    public int PathId { get; set; }
}