namespace Osmi.Game;

[PublicAPI]
public static class CoroutinePresets {
	public static IEnumerator FadeAudio(this AudioSource audio, float from, float to, float time) {
		if (time <= 0f) {
			throw new ArgumentOutOfRangeException(nameof(time));
		}

		if (from < 0f) {
			throw new ArgumentOutOfRangeException(nameof(from));
		}

		if (to < 0f) {
			throw new ArgumentOutOfRangeException(nameof(to));
		}

		float elapsedTime = 0f;

		while (elapsedTime <= time) {
			audio.volume = Mathf.Lerp(from, to, elapsedTime / time);
			yield return null;
			elapsedTime += Time.deltaTime;
		}
	}

	public static IEnumerator FadeAudio(this AudioSource audio, float to, float time) =>
		FadeAudio(audio, audio.volume, to, time);


	public static IEnumerator WaitForPlayFinished(this tk2dSpriteAnimator animator, string clip) {
		animator.Play(clip);
		yield return new WaitWhile(() => animator.IsPlaying(clip));
	}


	public static IEnumerator Move2DInTime(this Transform tf, Vector2 from, Vector2 to, float time) {
		if (time <= 0f) {
			throw new ArgumentOutOfRangeException(nameof(time));
		}

		float fromX = from.x, fromY = from.y;
		float toX = to.x, toY = to.y;
		float elapsedTime = 0f;

		while (elapsedTime < time) {
			tf.SetPosition2D(Mathf.Lerp(fromX, toX, elapsedTime / time), Mathf.Lerp(fromY, toY, elapsedTime / time));
			yield return null;
			elapsedTime += Time.deltaTime;
		}
	}
}
