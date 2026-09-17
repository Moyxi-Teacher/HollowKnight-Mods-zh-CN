using InControl;

namespace Osmi.Game;

public class KeyButtonActionSet : PlayerActionSet {
	public readonly PlayerAction key;
	public readonly PlayerAction button;

	public KeyButtonActionSet() {
		key = CreatePlayerAction("key");
		button = CreatePlayerAction("button");
	}

	public bool IsPressed => key.IsPressed || button.IsPressed;

	public bool WasPressed => key.WasPressed || button.WasPressed;

	public bool WasReleased => key.WasReleased || button.WasReleased;

	public bool WasRepeated => key.WasRepeated || button.WasRepeated;
}
