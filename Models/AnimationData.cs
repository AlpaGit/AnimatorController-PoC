using System.Text.Json.Serialization;

namespace AnimatorController.Models;

public class AnimationData
{
    [JsonPropertyName("m_GameObject")]
    public FileAsset FileAsset { get; set; }

    [JsonPropertyName("m_Enabled")]
    public int Enabled { get; set; }

    [JsonPropertyName("m_Script")]
    public FileAsset Script { get; set; }

    [JsonPropertyName("m_Name")]
    public string Name { get; set; }

    [JsonPropertyName("material")]
    public FileAsset Material { get; set; }

    [JsonPropertyName("uiMaterial")]
    public UiMaterial UiMaterial { get; set; }

    [JsonPropertyName("assetBundleName")]
    public string AssetBundleName { get; set; }

    [JsonPropertyName("defaultAnimationName")]
    public string DefaultAnimationName { get; set; }

    [JsonPropertyName("defaultAnimationLoops")]
    public int DefaultAnimationLoops { get; set; }

    [JsonPropertyName("defaultFrameRate")]
    public int DefaultFrameRate { get; set; }

    [JsonPropertyName("maxNodeCount")]
    public int MaxNodeCount { get; set; }

    [JsonPropertyName("exposedNodeNames")]
    public List<string> ExposedNodeNames { get; set; } = new List<string>();

    [JsonPropertyName("graphics")]
    public List<Graphic> Graphics { get; set; } = new List<Graphic>();

    [JsonPropertyName("animations")]
    public List<Animation> Animations { get; set; } = new List<Animation>();
}

public class Animation
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Data
{
    [JsonPropertyName("assetGuid")]
    public string AssetGuid { get; set; }
}