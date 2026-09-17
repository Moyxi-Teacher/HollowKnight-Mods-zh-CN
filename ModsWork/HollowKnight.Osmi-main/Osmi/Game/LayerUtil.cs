namespace Osmi.Game;

[PublicAPI]
public static class LayerUtil {
	public static int GetMask(this Layer layer) =>
		1 << (int) layer;

	public static int GetMask(params Layer[] layers) {
		int mask = 0;

		for (int i = 0; i < layers.Length; i++) {
			mask |= 1 << (int) layers[i];
		}

		return mask;
	}
}
