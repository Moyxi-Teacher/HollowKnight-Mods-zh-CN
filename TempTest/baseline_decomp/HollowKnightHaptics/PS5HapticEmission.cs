namespace HollowKnightHaptics;

public sealed class PS5HapticEmission : VibrationEmission
{
	internal readonly HapticPcm Pcm;

	internal double PositionFrames;

	private volatile bool isPlaying;

	private bool isLooping;

	private string tag;

	private VibrationTarget target;

	private readonly bool isRealtime;

	public override VibrationTarget Target
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return target;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			target = value;
		}
	}

	public override bool IsLooping
	{
		get
		{
			return isLooping;
		}
		set
		{
			isLooping = value;
		}
	}

	public override string Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public override bool IsRealtime => isRealtime;

	public override bool IsPlaying => isPlaying;

	public override float Time
	{
		get
		{
			return (float)(PositionFrames / 48000.0);
		}
		set
		{
			PositionFrames = value * 48000f;
		}
	}

	public PS5HapticEmission(HapticPcm pcm, float baseStrength, bool isLooping, string tag, VibrationTarget target, bool isRealtime)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((VibrationEmission)this)._002Ector();
		Pcm = pcm;
		this.isLooping = isLooping;
		this.tag = tag;
		this.target = target;
		this.isRealtime = isRealtime;
		((VibrationEmission)this).BaseStrength = baseStrength;
		isPlaying = true;
	}

	public override void Play()
	{
		isPlaying = true;
	}

	public override void Stop()
	{
		isPlaying = false;
	}

	internal void Advance(double frames)
	{
		PositionFrames += frames;
		if (PositionFrames >= (double)Pcm.FrameCount)
		{
			if (isLooping)
			{
				PositionFrames %= Pcm.FrameCount;
			}
			else
			{
				isPlaying = false;
			}
		}
	}
}
