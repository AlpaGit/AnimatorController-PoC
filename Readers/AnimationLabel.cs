namespace AnimatorController.Readers;

public class AnimationLabel
{
    public int Frame { get; private set; }
    public string Label { get; private set; }

    public AnimationLabel(int frame, string label)
    {
        Frame = frame;
        Label = label;
    }
}