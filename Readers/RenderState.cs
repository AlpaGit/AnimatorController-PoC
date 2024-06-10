using System.Runtime.InteropServices;

namespace AnimatorController.Readers;

[StructLayout(LayoutKind.Sequential, Size = 36)]
public struct RenderState
{
    public float m00;
    public float m01;
    public float m03;
    public float m10;
    public float m11;
    public float m13;
    public float multiplicativeColor;
    public float additiveColor;
    public short spriteIndex;
    public byte alpha;

    public unsafe int Compute(byte[] animationData, int dataPosition)
    {
        var num = (int)animationData[dataPosition];
        if ((num & 2) != 0)
            alpha = animationData[dataPosition + 1];
        dataPosition += 2;
        if ((num & 33) != 0)
        {
            fixed (byte* numPtr = &animationData[dataPosition])
                spriteIndex = *(short*)numPtr;
            dataPosition += 4;
        }

        if ((num & 4) != 0)
        {
            fixed (byte* numPtr = &animationData[dataPosition])
                multiplicativeColor = *(float*)numPtr;
            dataPosition += 4;
        }

        if ((num & 8) != 0)
        {
            fixed (byte* numPtr = &animationData[dataPosition])
                additiveColor = *(float*)numPtr;
            dataPosition += 4;
        }

        if ((num & 16) != 0)
        {
            fixed (byte* numPtr = &animationData[dataPosition])
            {
                m00 = *(float*)numPtr;
                m01 = *(float*)(numPtr + 4);
                m03 = *(float*)(numPtr + 8);
                m10 = *(float*)(numPtr + 12);
                m11 = *(float*)(numPtr + 16);
                m13 = *(float*)(numPtr + 20);
            }

            dataPosition += 24;
        }

        return dataPosition;
    }

    public void Reset()
    {
        m00 = 1f;
        m01 = 0.0f;
        m03 = 0.0f;
        m10 = 0.0f;
        m11 = 1f;
        m13 = 0.0f;
        spriteIndex = (short)-1;
        alpha = byte.MaxValue;
        multiplicativeColor = 16711422f;
        additiveColor = 8355711f;
    }
}