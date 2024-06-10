using System.Text.Json.Serialization;

namespace AnimatorController.Models;

public class FileAsset
{
    [JsonPropertyName("m_FileID")]
    public int FileID { get; set; }

    [JsonPropertyName("m_PathID")]
    public long PathID { get; set; }
}