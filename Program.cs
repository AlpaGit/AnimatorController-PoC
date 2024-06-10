using System.Text.Json;
using AnimatorController.Models;
using AnimatorController.Readers;

var spriteInfo = JsonSerializer.Deserialize<AnimationData>(File.ReadAllText("Examples/04007/data/04007.json"))!;

Console.WriteLine($"Sprite Info: {spriteInfo}");

var animAtlas = new AnimationAtlas("Examples/04007/atlas/04007-0.png", spriteInfo.Graphics);
var animMetadata = new AnimationMetadata("04007", File.ReadAllBytes("Examples/04007/animations/04007_idle1"));

// try to render the Frame 1
var animRenderer = new AnimationRenderer(spriteInfo.MaxNodeCount);

animRenderer.Render(animAtlas, animMetadata, 1);

Console.WriteLine("Done!");