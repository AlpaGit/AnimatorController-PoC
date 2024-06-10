namespace AnimatorController;

[Flags]
public enum NodeState : byte
{
    None = 0,
    SpriteIndex = 1,
    SpriteOpacity = 2,
    SpriteColorMultiply = 4,
    SpriteColorAdditive = 8,
    Matrix = 16, // 0x10
    CustomisationIndex = 32, // 0x20
}