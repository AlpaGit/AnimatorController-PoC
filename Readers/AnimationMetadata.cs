using System.Text;

namespace AnimatorController.Readers;

public class AnimationMetadata
{
    public readonly string Guid;
    public readonly ushort FrameCount;
    public readonly ushort NodeCount;
    public readonly ushort LabelCount;
    public NodeState CombinedNodeState;
    public readonly AnimationLabel[] Labels;
    public readonly int[] FrameDataPositions;
    
    public readonly byte[] Data;
    
    public AnimationMetadata(string guid, byte[] data)
    {
        FrameCount = BitConverter.ToUInt16(data, 0);
        NodeCount = BitConverter.ToUInt16(data, 2);
        LabelCount = BitConverter.ToUInt16(data, 4);
        CombinedNodeState = (NodeState) data[6];

        var startIndex = 8;
        Labels = new AnimationLabel[LabelCount];

        if (LabelCount > 0)
        {
            for (var i = 0; i < LabelCount; ++i)
            {
                var pos = BitConverter.ToUInt16(data, startIndex);
                var count = data[startIndex + 2];
                var label = Encoding.UTF8.GetString(data, startIndex + 3, count);
                Labels[i] = new AnimationLabel(pos, label);

                startIndex = (startIndex + 3 + count) % 2;
            }

            startIndex += startIndex % 4;
        }
        
        FrameDataPositions = new int[FrameCount];
        for (var i = 0; i < FrameCount; ++i)
        {
            FrameDataPositions[i] = BitConverter.ToInt32(data, startIndex);
            startIndex += 4;
        }
        
        Guid = guid;
        Data = data;
    }
}