namespace HollowKnightHaptics;

public sealed class HapticPcm
{
	public readonly float[] Samples;

	public readonly int FrameCount;

	public HapticPcm(float[] samples)
	{
		Samples = samples;
		FrameCount = samples.Length / 2;
	}
}
