namespace HollowKnightHaptics;

public sealed class PS5HapticEmission : VibrationEmission
{
	internal readonly HapticPcm Pcm;

	internal double PositionFrames;

	private volatile bool isPlaying;

	private bool isLooping;

	private string tag;

	private VibrationTarget target;

	// 当前游戏版本 VibrationEmission 无 Speed/Strength 属性，
	// 以本地字段替代（默认 1.0），供 MixBlock 使用。
	private float speed = 1f;
	private float strength = 1f;

	// 最大播放帧数；-1 表示不限时长（用于脚步等短脉冲限制，避免 2 秒长 WAV 连成持续震动）。
	private double maxFrames = -1.0;

	/// <summary>最大播放帧数（脚步短脉冲上限），-1 表示不限。</summary>
	internal double MaxFrames
	{
		get => maxFrames;
		set => maxFrames = value;
	}

	/// <summary>播放速度（采样率倍率），默认 1.0。</summary>
	internal float Speed
	{
		get => speed;
		set => speed = value;
	}

	/// <summary>震动强度倍率，默认 1.0。</summary>
	internal float Strength
	{
		get => strength;
		set => strength = value;
	}

	public override VibrationTarget Target
	{
		get => target;
		set => target = value;
	}

	public override bool IsLooping
	{
		get => isLooping;
		set => isLooping = value;
	}

	public override string Tag
	{
		get => tag;
		set => tag = value;
	}

	public override bool IsPlaying => isPlaying;

	// 当前游戏版本 VibrationEmission 无 Play() 方法（基类仅有 Stop()）；
	// 原版通过 ((VibrationEmission)e).Play() 激活已注册的 emission，
	// 新版改为直接调用 PS5HapticEmission.Play() 不可行，
	// 改为在 MixerPatch 拦截时直接调用 HapticEngine.Register() 来注册。
	// 此方法保留用于内部手动触发场景（如有需要可扩展）。
	public void Play()
	{
		isPlaying = true;
	}

	public PS5HapticEmission(HapticPcm pcm, float baseStrength, bool isLooping, string tag, VibrationTarget target)
	{
		Pcm = pcm;
		this.strength = baseStrength;
		this.isLooping = isLooping;
		this.tag = tag;
		this.target = target;
		isPlaying = true;
	}

	public override void Stop()
	{
		isPlaying = false;
	}

	internal void Advance(double frames)
	{
		PositionFrames += frames;
		// 时长上限：达到后强制停止（脚步短脉冲用，避免长尾持续震动）
		if (maxFrames > 0.0 && PositionFrames >= maxFrames)
		{
			isPlaying = false;
			return;
		}
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
